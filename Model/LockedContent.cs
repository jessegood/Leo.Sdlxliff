using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System;

namespace Leo.Sdlxliff.Model;

public class LockedContent : TranslationUnitContent
{
    /// <summary>
    /// For <c>x</c> elements. There are three possible content types:
    /// 1. Placeholder
    /// 2. StructureTag
    /// 3. LockedContent
    /// </summary>
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.LockedContent;

    /// <summary>
    /// <c>x</c> tags are self-closing, so this is always false.
    /// </summary>
    public override bool HasChildren => false;

    /// <summary>
    /// This id is used to look up the corresponding <c>tag</c> in the header.
    /// For locked content, this id starts with "locked" and refers to a previous translation unit.
    /// </summary>
    public override string Id { get; set; } = string.Empty;

    /// <summary>
    /// The locked translation unit that this tag refers to.
    /// </summary>
    public ITranslationUnit? LockedTranslationUnit { get; set; }

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    /// <summary>
    /// The <c>xid</c> is present when referring to locked content.
    /// Starts with "lockTU" for locked content.
    /// This is empty otherwise.
    /// </summary>
    public string XId { get; set; } = string.Empty;

    public override ITranslationUnitContent DeepCopy()
    {
        var transUnit = LockedTranslationUnit?.DeepCopy();
        return new LockedContent()
        {
            Id = Id,
            XId = transUnit?.TranslationUnitId ?? string.Empty,
            LockedTranslationUnit = transUnit
        };
    }

    public override string ToString()
    {
        return @$"<x id=""{Id}"" xid=""{XId}""/>";
    }
}
