using System.Threading.Tasks;

namespace SummonMyStrength.Maui.ComponentsOld.Common.DragDrop;

/// <summary>
/// An interface for the features of a dropsite.
/// </summary>
public interface IDropsite
{
    /// <summary>
    /// A method called when the dragged content leaves the dropsite's area.
    /// </summary>
    void OnLeave();

    /// <summary>
    /// A method called when the object is dropped while inside the dropsite's area.
    /// </summary>
    /// <param name="droppedObject">The object that was dropped.</param>
    /// <returns>A task for the action.</returns>
    Task OnDropAsync(object droppedObject);

    /// <summary>
    /// A method called when a compatible object starts being dragged.
    /// </summary>
    void OnDragStart();

    /// <summary>
    /// A method called wehn a compatible object stops being dragged.
    /// </summary>
    void OnDragStop();

    /// <summary>
    /// A method used to determine if the object being dragged is compatible with this dropsite.
    /// </summary>
    /// <param name="groupKey">The group key used to ensure objects cannot be dragged from other scopes.</param>
    /// <param name="key">The object being dragged.</param>
    /// <returns>Whether or not this is a valid dropsite for the drag-drop operation.</returns>
    bool IsValidDropSiteFor(object groupKey, object key);
}
