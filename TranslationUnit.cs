using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff;

public sealed class TranslationUnit : ITranslationUnit
{
    /// <summary>
    /// Stores the segments actually visible in the editor.
    /// The segments are always surrounded by <mrk mtype="seg"></mrk>
    /// </summary>
    private readonly IDictionary<string, ISegmentPair> segmentPairs;

    private TranslationUnit(SegmentationSource segmentSource, Target target, ContextInformation? contextInformation,
                            bool isLocked, bool isStructure, LockType lockType, string translationUnitId,
                            Source source, Translate translate, IDictionary<string, ISegmentPair> segmentPairs)
    {
        SegmentationSource = segmentSource;
        Target = target;
        ContextInformation = contextInformation;
        IsLocked = isLocked;
        IsStructure = isStructure;
        LockType = lockType;
        TranslationUnitId = translationUnitId;
        Source = source;
        Translate = translate;
        this.segmentPairs = segmentPairs;
        HasSegmentPairs = segmentPairs.Count > 0;
    }

    /// <summary>
    /// When <c>trans-unit</c> is part of a group.
    /// This holds the context information for the group.
    /// </summary>
    public ContextInformation? ContextInformation { get; }

    public bool HasSegmentPairs { get; }

    /// <summary>
    /// True when the translation unit is locked content.
    /// This is not the same as the locked attribute for segments!
    /// </summary>
    public bool IsLocked { get; }

    /// <summary>
    /// <c>true</c> when the translation is a structure
    /// </summary>
    public bool IsStructure { get; }

    /// <summary>
    /// Indicates the lock types for this translation unit.
    /// For a structure paragraph unit, the Structure flag is set.
    /// </summary>
    public LockType LockType { get; }

    /// <summary>
    /// A <c>seg-source</c> for this translation unit.
    /// </summary>
    public SegmentationSource SegmentationSource { get; }

    /// <summary>
    /// The <c>source</c> element of this translation unit.
    /// </summary>
    public Source Source { get; }

    /// <summary>
    /// The <c>target</c> element of this translation unit
    /// </summary>
    public Target Target { get; }

    /// <summary>
    /// Specifies if this is translation unit is translatable or not
    /// </summary>
    public Translate Translate { get; }

    /// <summary>
    /// Globally unique identifier of the translation unit
    /// </summary>
    public string TranslationUnitId { get; set; }

    /// <summary>
    /// Gets a specific segment pair by id
    /// </summary>
    /// <returns>SegmentPair</returns>
    public ISegmentPair GetSegmentPairById(string id)
    {
        return segmentPairs[id];
    }

    /// <summary>
    /// Gets the segment pairs as a collection
    /// </summary>
    /// <returns>ICollection<SegmentPair></returns>
    public ICollection<ISegmentPair> GetSegmentPairs()
    {
        return segmentPairs.Values;
    }

    internal sealed class TranslationUnitBuilder
    {
        private readonly ContextInformation? contextInformation;
        private readonly IDictionary<string, IList<Entry>> repetitionDefinitions;
        private readonly IDictionary<string, ISegmentPair> segmentPairs = new Dictionary<string, ISegmentPair>();
        private bool isLocked = false;
        private LockType lockType = LockType.Unlocked;
        private SegmentationSource segmentSource = new SegmentationSource();
        private Source source = new Source();
        private Target target = new Target();
        private Translate translate = Translate.Yes;
        private string translationUnitId = string.Empty;

        internal TranslationUnitBuilder(IDictionary<string, IList<Entry>> repetitionDefinitions, ContextInformation? contextInformation = null)
        {
            this.repetitionDefinitions = repetitionDefinitions;
            this.contextInformation = contextInformation;
        }

        internal bool IsStructure
        {
            get; private set;
        }

        internal ITranslationUnit Build()
        {
            var translationUnit = new TranslationUnit(segmentSource, target, contextInformation, isLocked,
                                       IsStructure, lockType, translationUnitId, source, translate, segmentPairs);

            // Set the parent of the segmentpairs
            foreach (var segmentPair in segmentPairs.Values)
            {
                segmentPair.Parent = translationUnit;
            }

            return translationUnit;
        }

        internal void CheckIfStructure()
        {
            if ((translate == Translate.No)
                && IsUnlockedOrStructure())
            {
                this.lockType = LockType.Structure;
                IsStructure = true;
            }
        }

        internal void SetLockType(LockType lockType)
        {
            this.lockType = lockType;
        }

        internal void SetSegmentationSource(SegmentationSource segmentSource)
        {
            this.segmentSource = segmentSource;
        }

        internal void SetSource(Source source)
        {
            this.source = source;
        }

        internal void SetSourceSegment(string id, ISegment sourceSegment, SegmentDefinition segmentDefinition)
        {
            var segmentPair = new SegmentPair()
            {
                SourceSegment = sourceSegment,
                Id = id,
                ConfirmationLevel = segmentDefinition.ConfirmationLevel,
                Locked = segmentDefinition.Locked,
                Origin = segmentDefinition.Origin,
                OriginSystem = segmentDefinition.OriginSystem,
                Percent = segmentDefinition.Percent,
                Previous = segmentDefinition.Previous,
                RepetitionId = segmentDefinition.RepetitionId,
                RepetitionInfo = GetRepetitionInfo(segmentDefinition.RepetitionId),
                SegmentDefinitionId = segmentDefinition.Id,
                StructMatch = segmentDefinition.StructMatch,
                TextMatch = segmentDefinition.TextMatch,
                Values = segmentDefinition.Values
            };

            segmentPairs.Add(id, segmentPair);
        }

        internal void SetTarget(Target target)
        {
            this.target = target;
        }

        internal void SetTargetSegment(string id, ISegment targetSegment, SegmentDefinition segmentDefinition)
        {
            segmentPairs[id].TargetSegment = targetSegment;
            segmentPairs[id].Id = id;
            segmentPairs[id].ConfirmationLevel = segmentDefinition.ConfirmationLevel;
            segmentPairs[id].Locked = segmentDefinition.Locked;
            segmentPairs[id].Origin = segmentDefinition.Origin;
            segmentPairs[id].OriginSystem = segmentDefinition.OriginSystem;
            segmentPairs[id].Percent = segmentDefinition.Percent;
            segmentPairs[id].Previous = segmentDefinition.Previous;
            segmentPairs[id].RepetitionId = segmentDefinition.RepetitionId;
            segmentPairs[id].SegmentDefinitionId = segmentDefinition.Id;
            segmentPairs[id].StructMatch = segmentDefinition.StructMatch;
            segmentPairs[id].TextMatch = segmentDefinition.TextMatch;
            segmentPairs[id].Values = segmentDefinition.Values;
        }

        internal void SetTranslateAttribute(Translate translate)
        {
            this.translate = translate;
        }

        internal void SetTranslationUnitId(string translationUnitId)
        {
            // The prefix "lockTU_" on the id indicates whether this is a locked paragraph unit
            // A locked paragraph unit indicates it should not be changed during translation
            // We make sure the prefix isn't want that the tool added
            if (!translationUnitId.StartsWith("lockTU_Leo") && translationUnitId.StartsWith("lockTU_"))
            {
                isLocked = true;
            }

            this.translationUnitId = translationUnitId;
        }

        private RepetitionInfo GetRepetitionInfo(string repID)
        {
            if (!string.IsNullOrEmpty(repID) &&
                repetitionDefinitions.TryGetValue(repID, out IList<Entry>? values))
            {
                if (values[0].Tu == translationUnitId)
                {
                    return RepetitionInfo.FirstOccurence;
                }

                return RepetitionInfo.NotFirstOccurence;
            }

            return RepetitionInfo.Unique;
        }

        private bool IsUnlockedOrStructure()
        {
            return lockType == LockType.Unlocked ||
                   lockType.HasFlag(LockType.Structure);
        }
    }
}
