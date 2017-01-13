using System;

namespace Showaspnetcore.Interface
{
    public interface IIntegerListItem
    {
        Guid Value { get; }
        string Text { get; }
    }
}