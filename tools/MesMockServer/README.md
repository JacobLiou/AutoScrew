# MesMockServer

本地 FAT 占位 MES HTTP 服务（契约见 `doc/DATA_AND_TRACE.md` § MES HTTP v1）。

```bash
dotnet run --project tools/MesMockServer
```

默认监听 `http://localhost:5080/`。HMI Mes 页关闭 Mock 并将基址设为该 URL 即可联调。
