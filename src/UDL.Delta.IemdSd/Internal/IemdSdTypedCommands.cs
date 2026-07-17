using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class IemdSdTypedCommands
{
    private readonly IIemdSdCommandExecutor _executor;
    private readonly IModbusTransport _transport;
    private readonly int _toolIndex;

    public IemdSdTypedCommands(IIemdSdCommandExecutor executor, IModbusTransport transport, int toolIndex)
    {
        _executor = executor;
        _transport = transport;
        _toolIndex = toolIndex;
    }

    public Task<ModbusCommandResult> ExecuteAsync(ModbusCommandInvocation invocation, CancellationToken ct) =>
        _executor.ExecuteAsync(invocation, ct);

    // --- Phase A: Runtime / Source / Status / Export / History ---

    public Task WriteBarcodeAsync(string barcode, CancellationToken ct)
    {
        var payload = AsciiWords(barcode, 40);
        return _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload((int)ModbusFunctionCode.Write_barcode_string, payload),
            ct);
    }

    public async Task<string> ReadBarcodeAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload((int)ModbusFunctionCode.Read_barcode_string, 40),
            ct).ConfigureAwait(false);
        return ReadAscii(result.ReadPayload);
    }

    public Task ClearErrorsAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_clear_all_errors, ct);

    public Task ResetOperationProgressAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_reset_operation_progress, ct);

    public Task ForcePreviousStepAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_force_execute_previous_step, ct);

    public Task ForceNextStepAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_force_execute_next_step, ct);

    public Task RestrictLooseningAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_restrict_loosening_operation, ct);

    public Task ClearTighteningNokCountAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_clear_single_screw_tightening_NOK, ct);

    public Task ClearLooseningNokCountAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_clear_single_screw_loosening_NOK, ct);

    public Task ResetOperationTimeAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_reset_operation_time, ct);

    public Task ResetOperationStatusAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_reset_operation_status, ct);

    public Task WriteSourceModeAsync(int operatingMode, int switchingMethod, CancellationToken ct) =>
        WriteSourceModeAsync(_toolIndex, operatingMode, switchingMethod, ct);

    public Task WriteSourceModeAsync(int toolIndex, int operatingMode, int switchingMethod, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_operating_mode_switching_method_source,
                word2: toolIndex,
                word3: operatingMode,
                word4: switchingMethod),
            ct);

    public async Task<TighteningSourceSnapshot> ReadSourceModeAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload((int)ModbusFunctionCode.Read_operating_mode_switching_method_source, 4),
            ct).ConfigureAwait(false);
        var w = result.ReadPayload ?? [];
        return new TighteningSourceSnapshot
        {
            ToolIndex = w.ElementAtOrDefault(0),
            OperatingMode = w.ElementAtOrDefault(1),
            SwitchingMethod = w.ElementAtOrDefault(2),
        };
    }

    public Task WriteSourceContentAsync(TighteningSourceContentCore content, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(content);
        // 手册 #301：CA=工具号，CB=切换方式 ID（来源 ID）
        var toolIndex = content.ToolIndex is 0 or 1 ? content.ToolIndex : _toolIndex;
        var sourceId = content.SwitchingMethodId > 0 ? content.SwitchingMethodId : 1;
        var raw = new int[TighteningSequenceRegisterMap.SourceContentWordCount];
        TighteningSourceCodec.ApplyContentToRaw(raw, content);
        return _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                (int)ModbusFunctionCode.Write_contents_single_source,
                raw,
                word2: toolIndex,
                word3: sourceId),
            ct);
    }

    public async Task<TighteningSourceSnapshot> ReadSourceContentAsync(int sourceId, CancellationToken ct)
    {
        // 手册 #351：与 #301 同布局；mailbox word2=切换方式 ID
        var wordCount = (uint)TighteningSequenceRegisterMap.SourceContentWordCount;
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(
                (int)ModbusFunctionCode.Read_contents_single_source,
                wordCount,
                word2: sourceId),
            ct).ConfigureAwait(false);

        var raw = new int[TighteningSequenceRegisterMap.SourceContentWordCount];
        var payload = result.ReadPayload ?? [];
        Array.Copy(payload, raw, Math.Min(payload.Length, raw.Length));

        var content = TighteningSourceCodec.ExtractContentFromRaw(raw);
        content.SwitchingMethodId = sourceId > 0 ? sourceId : 1;
        content.ToolIndex = _toolIndex;
        return TighteningSourceSnapshot.FromContent(content);
    }

    public Task SwitchSequenceUnderManualAsync(int sequenceId, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_switch_sequence_under_manual_setting,
                word2: _toolIndex,
                word3: sequenceId),
            ct);

    public Task WriteSourceSwitchingMethodAsync(int method, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_switching_method_source,
                word2: method),
            ct);

    public async Task<int> ReadSourceSwitchingMethodAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload((int)ModbusFunctionCode.Read_switching_method_source, 1),
            ct).ConfigureAwait(false);
        return result.ReadPayload?.ElementAtOrDefault(0) ?? 0;
    }

    public async Task<TighteningIndicatorStatus> ReadIndicatorStatusAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload((int)ModbusFunctionCode.Read_tightening_status_indicator, 1),
            ct).ConfigureAwait(false);
        return new TighteningIndicatorStatus { IndicatorCode = result.ReadPayload?.ElementAtOrDefault(0) ?? 0 };
    }

    public Task SetPerScrewExportAsync(PerScrewExportMode mode, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_file_format_exported_result_each,
                word2: (int)mode),
            ct);

    public async Task<PerScrewExportMode> ReadPerScrewExportAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(
                (int)ModbusFunctionCode.Read_file_format_exported_result_each,
                1),
            ct).ConfigureAwait(false);
        return (PerScrewExportMode)(result.ReadPayload?.ElementAtOrDefault(0) ?? 0);
    }

    public async Task<DefaultTorqueUnit> ReadDefaultTorqueUnitAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(
                (int)ModbusFunctionCode.Read_default_torque_unit,
                1),
            ct).ConfigureAwait(false);
        var raw = result.ReadPayload?.ElementAtOrDefault(0) ?? (int)DefaultTorqueUnit.KgfCm;
        return raw is >= 0 and <= 3
            ? (DefaultTorqueUnit)raw
            : DefaultTorqueUnit.KgfCm;
    }

    public Task<int[]> ReadErrorReportAsync(uint reportId, uint wordCount, CancellationToken ct) =>
        ReadHistoryAsync((int)ModbusFunctionCode.Find_read_error_report_entries, reportId, wordCount, ct);

    public Task<int[]> ReadWarningReportAsync(uint reportId, uint wordCount, CancellationToken ct) =>
        ReadHistoryAsync((int)ModbusFunctionCode.Find_read_warning_report_entries, reportId, wordCount, ct);

    public Task<int[]> ReadButtonReportAsync(uint reportId, uint wordCount, CancellationToken ct) =>
        ReadHistoryAsync((int)ModbusFunctionCode.Find_read_button_report_entries, reportId, wordCount, ct);

    public Task<int[]> ReadSortedProductionReportsAsync(uint wordCount, CancellationToken ct) =>
        ReadPayloadOnly((int)ModbusFunctionCode.Read_sorted_production_reports, wordCount, ct);

    // --- Phase B: Parameter+ / Sequence ---

    public Task DeleteParameterAsync(int parameterId, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_delete_parameter,
                word2: _toolIndex,
                word3: parameterId),
            ct);

    public Task QuickSetParameterAsync(int parameterId, int[] payload, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                (int)ModbusFunctionCode.Write_set_tightening_parameter_through_quick,
                payload,
                word2: _toolIndex,
                word3: parameterId),
            ct);

    public async Task<ParameterListSnapshot> ListParametersAsync(uint wordCount, CancellationToken ct)
    {
        var words = await ReadPayloadWithToolIndex(
                (int)ModbusFunctionCode.Read_created_sets_tightening_parameters,
                wordCount,
                ct)
            .ConfigureAwait(false);
        return new ParameterListSnapshot { RawWords = words };
    }

    public async Task<ParameterListSnapshot> ListParametersForToolAsync(int toolIndex, uint wordCount, CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
                ModbusCommandInvocation.WithReadPayload(
                    (int)ModbusFunctionCode.Read_created_sets_tightening_parameters,
                    wordCount,
                    word2: toolIndex),
                ct)
            .ConfigureAwait(false);
        return new ParameterListSnapshot { RawWords = result.ReadPayload ?? [] };
    }

    public async Task<ParameterListSnapshot> ListParametersWithoutToolIndexAsync(uint wordCount, CancellationToken ct)
    {
        // Legacy: mailbox word2=0 ⇒ tool 0 (same as ListParametersForToolAsync(0)).
        var words = await ReadPayloadOnly(
                (int)ModbusFunctionCode.Read_created_sets_tightening_parameters,
                wordCount,
                ct)
            .ConfigureAwait(false);
        return new ParameterListSnapshot { RawWords = words };
    }

    public Task WriteSequenceAsync(TighteningSequenceTemplate template, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(template);
        return _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                (int)ModbusFunctionCode.Write_sequence,
                template.RawBlock,
                word2: template.SequenceId),
            ct);
    }

    public async Task<TighteningSequenceTemplate> ReadSequenceAsync(int sequenceId, CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(
                (int)ModbusFunctionCode.Read_sequence,
                (uint)TighteningSequenceTemplate.SequenceBlockWordCount,
                word2: sequenceId),
            ct).ConfigureAwait(false);
        return new TighteningSequenceTemplate
        {
            SequenceId = sequenceId,
            RawBlock = result.ReadPayload ?? TighteningSequenceTemplate.CreateEmptyRawBlock(),
        };
    }

    public Task DeleteSequenceAsync(int sequenceId, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_delete_sequence,
                word2: sequenceId),
            ct);

    public Task WriteNavigatorCoordinatesAsync(int sequenceId, int[] payload, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                (int)ModbusFunctionCode.Write_navigator_coordinates,
                payload,
                word2: sequenceId),
            ct);

    public Task<int[]> ReadNavigatorCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken ct) =>
        ReadPayloadWithId((int)ModbusFunctionCode.Read_navigator_coordinates, sequenceId, wordCount, ct);

    public Task WriteNavigatorImageCodesAsync(int sequenceId, int[] payload, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                (int)ModbusFunctionCode.Write_navigator_image_codes,
                payload,
                word2: sequenceId),
            ct);

    public Task<int[]> ReadNavigatorImageCodesAsync(int sequenceId, uint wordCount, CancellationToken ct) =>
        ReadPayloadWithId((int)ModbusFunctionCode.Read_navigator_image_codes, sequenceId, wordCount, ct);

    public Task WritePositioningArmCoordinatesAsync(int sequenceId, int[] payload, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                (int)ModbusFunctionCode.Write_coordinates_positioning_arm,
                payload,
                word2: sequenceId),
            ct);

    public Task<int[]> ReadPositioningArmCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken ct) =>
        ReadPayloadWithId((int)ModbusFunctionCode.Read_coordinates_positioning_arm, sequenceId, wordCount, ct);

    public Task<int[]> ListSequencesAsync(uint wordCount, CancellationToken ct) =>
        ReadPayloadOnly((int)ModbusFunctionCode.Read_created_sets_tightening_sequences, wordCount, ct);

    // --- Phase C/D: System / Tool / Maintenance / Operating status ---

    public Task LoginAsync(int role, int passwordHash, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_request_permissions_login,
                word2: role,
                word3: passwordHash),
            ct);

    public Task LogoutAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_request_permissions_logout, ct);

    public async Task<FirmwareVersionInfo> ReadFirmwareVersionAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload((int)ModbusFunctionCode.Read_firmware_version, 20),
            ct).ConfigureAwait(false);
        var w = result.ReadPayload ?? [];
        return new FirmwareVersionInfo
        {
            ControllerVersion = ReadAscii(w.AsSpan(0, Math.Min(10, w.Length))),
            BiosVersion = w.Length > 10 ? ReadAscii(w.AsSpan(10, Math.Min(10, w.Length - 10))) : string.Empty,
        };
    }

    public async Task<ToolInformationSnapshot> ReadToolInformationAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(
                (int)ModbusFunctionCode.Read_tool_information,
                30,
                word2: _toolIndex),
            ct).ConfigureAwait(false);
        return new ToolInformationSnapshot
        {
            ToolIndex = _toolIndex,
            RawWords = result.ReadPayload ?? [],
        };
    }

    public Task ActivateToolAsync(bool enabled, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_activate_tool,
                word2: _toolIndex,
                word3: enabled ? 1 : 0),
            ct);

    public Task CalibrateToolAsync(CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_calibrate_tool,
                word2: _toolIndex),
            ct);

    public Task ClearProductionReportsAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Clear_production_report_entries, ct);

    public Task ClearErrorWarningReportsAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Clear_error_warning_report_entries, ct);

    public Task ClearProductionReportFilesAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Clear_production_report_files, ct);

    public async Task<OperatingStatusSnapshot> ReadOperatingStatusAsync(CancellationToken ct)
    {
        var words = await _transport.ReadHoldingAsync(
                ModbusRegisterMap.OperatingStatusStart,
                ModbusRegisterMap.OperatingStatusWordCount,
                ct)
            .ConfigureAwait(false);

        var reportLow = await _transport.ReadSingleAsync(ModbusRegisterMap.ReportIdLow, ct).ConfigureAwait(false);
        var reportHigh = await _transport.ReadSingleAsync(ModbusRegisterMap.ReportIdHigh, ct).ConfigureAwait(false);
        var ready = await _transport.ReadSingleAsync(ModbusRegisterMap.Ready, ct).ConfigureAwait(false);
        var finish = await _transport.ReadSingleAsync(ModbusRegisterMap.TighteningFinish, ct).ConfigureAwait(false);

        static int At(int[] w, int hex) => w[hex - ModbusRegisterMap.OperatingStatusStart];

        return new OperatingStatusSnapshot
        {
            TighteningResult = (DeviceTighteningStatus)(ushort)At(words, 0x26),
            FinalTorqueNm = At(words, 0x2B) / 1000f,
            CompTorqueNm = At(words, 0x2C) / 1000f,
            TotalAngle = At(words, 0x24),
            ReportId = (uint)(reportHigh * 65536 + (ushort)reportLow),
            Ready = ready,
            TighteningFinish = finish,
        };
    }

    public Task LimitTighteningAsync(CancellationToken ct) =>
        ExecuteMailbox((int)ModbusFunctionCode.Write_restrict_tightening_operation, ct);

    public Task SetAutoLockAsync(bool enabled, CancellationToken ct) =>
        _executor.ExecuteAsync(
            ModbusCommandInvocation.MailboxOnly(
                (int)ModbusFunctionCode.Write_prohibit_tool_operation_after_each,
                word2: enabled ? 1 : 0),
            ct);

    public async Task<int> ReadCurveSampleRateAsync(CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            new ModbusCommandInvocation
            {
                FunctionCode = (int)ModbusFunctionCode.Read_sampling_rate_curves,
                MailboxWords = CommandMailbox.CreateRequest((int)ModbusFunctionCode.Read_sampling_rate_curves),
                ReadbackFromCommandRequest = true,
            },
            ct).ConfigureAwait(false);
        return result.ReadbackValue ?? 0;
    }

    public Task SwitchParameterAsync(int parameterId, uint screwCount, CancellationToken ct)
    {
        var req = ModbusCommandInvocation.MailboxOnly(
            (int)ModbusFunctionCode.Write_switch_parameter_under_manual_setting,
            word2: _toolIndex,
            word3: parameterId,
            word4: (int)(screwCount % 65536),
            word5: (int)(screwCount / 65536));
        return _executor.ExecuteAsync(req, ct);
    }

    private Task ExecuteMailbox(int code, CancellationToken ct) =>
        _executor.ExecuteAsync(ModbusCommandInvocation.MailboxOnly(code), ct);

    private async Task<int[]> ReadHistoryAsync(int code, uint reportId, uint wordCount, CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReportId(code, reportId, wordCount),
            ct).ConfigureAwait(false);
        return result.ReadPayload ?? [];
    }

    private async Task<int[]> ReadPayloadOnly(int code, uint wordCount, CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(code, wordCount),
            ct).ConfigureAwait(false);
        return result.ReadPayload ?? [];
    }

    private async Task<int[]> ReadPayloadWithId(int code, int id, uint wordCount, CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(code, wordCount, word2: id),
            ct).ConfigureAwait(false);
        return result.ReadPayload ?? [];
    }

    private async Task<int[]> ReadPayloadWithToolIndex(int code, uint wordCount, CancellationToken ct)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(code, wordCount, word2: _toolIndex),
            ct).ConfigureAwait(false);
        return result.ReadPayload ?? [];
    }

    private static int[] AsciiWords(string text, int wordCount)
    {
        var bytes = System.Text.Encoding.ASCII.GetBytes(text ?? string.Empty);
        var words = new int[wordCount];
        for (var i = 0; i < wordCount; i++)
        {
            var hi = i * 2 < bytes.Length ? bytes[i * 2] : (byte)0;
            var lo = i * 2 + 1 < bytes.Length ? bytes[i * 2 + 1] : (byte)0;
            words[i] = (hi << 8) | lo;
        }

        return words;
    }

    private static string ReadAscii(int[]? words)
    {
        if (words is null || words.Length == 0)
            return string.Empty;
        return ReadAscii(words.AsSpan());
    }

    private static string ReadAscii(ReadOnlySpan<int> words)
    {
        var bytes = new List<byte>(words.Length * 2);
        foreach (var word in words)
        {
            bytes.Add((byte)((word >> 8) & 0xFF));
            bytes.Add((byte)(word & 0xFF));
        }

        var end = bytes.IndexOf(0);
        if (end < 0)
            end = bytes.Count;
        return System.Text.Encoding.ASCII.GetString(bytes.ToArray(), 0, end).Trim();
    }
}
