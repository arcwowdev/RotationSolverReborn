﻿using Dalamud.Interface.Utility.Raii;
using ECommons.Logging;

namespace RotationSolver.UI;

internal class CollapsingHeaderGroup
{
    private readonly Dictionary<Func<string>, Action> _headers;
    private int _openedIndex = -1;

    public float HeaderSize { get; set; } = 24;

    public CollapsingHeaderGroup(Dictionary<Func<string>, Action> headers)
    {
        _headers = headers ?? throw new ArgumentNullException(nameof(headers));
    }

    public void AddCollapsingHeader(Func<string> name, Action action)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _headers[name] = action;
    }

    public void RemoveCollapsingHeader(Func<string> name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        _ = _headers.Remove(name);
    }

    public void ClearCollapsingHeader()
    {
        _headers.Clear();
    }

    public void Draw()
    {
        int index = -1;
        foreach (KeyValuePair<Func<string>, Action> header in _headers)
        {
            index++;

            if (header.Key is null || header.Value is null)
            {
                continue;
            }

            string name = header.Key();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            try
            {
                ImGui.Spacing();
                ImGui.Separator();
                bool selected = index == _openedIndex;
                bool changed = false;
                using (ImRaii.Font font = ImRaii.PushFont(FontManager.GetFont(18)))
                {
                    changed = ImGui.Selectable(name, selected, ImGuiSelectableFlags.DontClosePopups);
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }
                if (changed)
                {
                    _openedIndex = selected ? -1 : index;
                }
                if (selected)
                {
                    header.Value();
                }
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"An error occurred while drawing the header: {ex.Message}");
            }
        }
    }
}