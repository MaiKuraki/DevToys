﻿namespace DevToys.Api;

/// <summary>
/// A component that represents a drop zone and selector for files, folders, images coming from the clipboard.
/// </summary>
public interface IUIFileSelector : IUIElement
{
    /// <summary>
    /// Gets the list of absolute path to the file(s) the user selected.
    /// </summary>
    /// <remarks>The items contain a stream. It won't be disposed automatically. It is important to dispose the stream yourself, when not needed anymore</remarks>
    PickedFile[] SelectedFiles { get; }

    /// <summary>
    /// Gets whether the user is allowed to select more than one file.
    /// </summary>
    bool CanSelectManyFiles { get; }

    /// <summary>
    /// Gets the list of file extensions allowed to be selected. An empty list of file extensions indicates any kind of file.
    /// </summary>
    string[] AllowedFileExtensions { get; }

    /// <summary>
    /// Gets the action to run when the user selected file(s).
    /// </summary>
    /// <remarks>The items contain a stream. It won't be disposed automatically. It is important to dispose the stream yourself, when not needed anymore</remarks>
    Func<PickedFile[], ValueTask>? OnFilesSelectedAction { get; }

    /// <summary>
    /// Raised when <see cref="CanSelectManyFiles"/> is changed.
    /// </summary>
    event EventHandler? CanSelectManyFilesChanged;

    /// <summary>
    /// Raised when <see cref="AllowedFileExtensions"/> is changed.
    /// </summary>
    event EventHandler? AllowedFileExtensionsChanged;
}

[DebuggerDisplay($"Id = {{{nameof(Id)}}}, SelectedFiles = {{{nameof(SelectedFiles)}}}")]
internal sealed class UIFileSelector : UIElement, IUIFileSelector
{
    private PickedFile[] _selectedFiles = Array.Empty<PickedFile>();
    private bool _allowMultipleFileSelection;
    private string[] _allowedFileExtensions = Array.Empty<string>();

    internal UIFileSelector(string? id)
        : base(id)
    {
    }

    public PickedFile[] SelectedFiles
    {
        get => _selectedFiles;
        internal set
        {
            SetPropertyValue(ref _selectedFiles, value, null);
            OnFilesSelectedAction?.Invoke(_selectedFiles);
        }
    }

    public bool CanSelectManyFiles
    {
        get => _allowMultipleFileSelection;
        internal set => SetPropertyValue(ref _allowMultipleFileSelection, value, CanSelectManyFilesChanged);
    }

    public string[] AllowedFileExtensions
    {
        get => _allowedFileExtensions;
        internal set => SetPropertyValue(ref _allowedFileExtensions, value, AllowedFileExtensionsChanged);
    }

    public Func<PickedFile[], ValueTask>? OnFilesSelectedAction { get; internal set; }

    public event EventHandler? CanSelectManyFilesChanged;
    public event EventHandler? AllowedFileExtensionsChanged;
}

public static partial class GUI
{
    /// <summary>
    /// Create a component that represents a drop zone and selector for files, folders, images coming from the clipboard.
    /// </summary>
    public static IUIFileSelector FileSelector()
    {
        return FileSelector(null);
    }

    /// <summary>
    /// Create a component that represents a drop zone and selector for files, folders, images coming from the clipboard.
    /// </summary>
    /// <param name="id">An optional unique identifier for this UI element.</param>
    public static IUIFileSelector FileSelector(string? id)
    {
        return new UIFileSelector(id);
    }

    /// <summary>
    /// Sets selected files.
    /// </summary>
    /// <remarks>The items contain a stream. It won't be disposed automatically. It is important to dispose the stream yourself, when not needed anymore</remarks>
    public static IUIFileSelector WithFiles(this IUIFileSelector element, params PickedFile[] files)
    {
        ((UIFileSelector)element).SelectedFiles = files;
        return element;
    }

    /// <summary>
    /// Sets the action to run when the user selected file(s).
    /// </summary>
    /// <remarks>The items contain a stream. It won't be disposed automatically. It is important to dispose the stream yourself, when not needed anymore</remarks>
    public static IUIFileSelector OnFilesSelected(this IUIFileSelector element, Func<PickedFile[], ValueTask>? actionOnFilesSelected)
    {
        ((UIFileSelector)element).OnFilesSelectedAction = actionOnFilesSelected;
        return element;
    }

    /// <summary>
    /// Sets the list of file extensions allowed to be selected. An empty list of file extensions indicates any kind of file.
    /// </summary>
    public static IUIFileSelector LimitFileTypesTo(this IUIFileSelector element, params string[] fileExtensions)
    {
        ((UIFileSelector)element).AllowedFileExtensions = fileExtensions;
        return element;
    }

    /// <summary>
    /// Allows the user to select only one file at once.
    /// </summary>
    public static IUIFileSelector CanSelectOneFile(this IUIFileSelector element)
    {
        ((UIFileSelector)element).CanSelectManyFiles = false;
        return element;
    }

    /// <summary>
    /// Allows the user to select multiple files at once.
    /// </summary>
    public static IUIFileSelector CanSelectManyFiles(this IUIFileSelector element)
    {
        ((UIFileSelector)element).CanSelectManyFiles = true;
        return element;
    }
}
