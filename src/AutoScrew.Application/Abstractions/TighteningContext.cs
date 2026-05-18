namespace AutoScrew.Application.Abstractions;

/// <summary>单次拧紧前由应用层传入的工位/工艺上下文。</summary>
public sealed record TighteningContext(int PositionIndex, int ControllerParameterId = 1);
