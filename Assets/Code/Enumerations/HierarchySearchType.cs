using System;

[Flags]
public enum HierarchySearchType
{
    Parent = 1,
    Ancestors = 2,
    Children = 4,
    Descendants = 8,
    Siblings = 16,
    Niblings = 32,
    All = 64
}
