namespace Leo.Sdlxliff.Model.Common;

public enum RevisionType
{
    /// <summary>
    /// Content that has been inserted
    /// </summary>
    Insert,

    /// <summary>
    /// Content that has been deleted
    /// </summary>
    Delete,

    /// <summary>
    /// Content that has feedback associated with it
    /// </summary>
    FeedbackComment,

    /// <summary>
    /// Content which has been added for feedback
    /// </summary>
    FeedbackAdded,

    /// <summary>
    /// Content which has been deleted for feedback
    /// </summary>
    FeedbackDeleted,

    /// <summary>
    /// Content that has not been changed
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is equivalent to not annotating content with a revision marker.
    /// </para>
    /// </remarks>
    Unchanged
}
