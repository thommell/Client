using System;
using System.Collections.Generic;
using Godot;


// This class is right now responsible to changing UI state and changing the current filter state for the maplist.
// I have not fully removed everything (as this was just a simple ctrlc+ctrlv from SortMenuButton, some stuff is still incorrect.
public partial class FilterMenuButton : Button, ISkinnable
{
    [Export]
    private Control panel;

    [Export]
    private Button order;

    [Export]
    private Label orderLabel;

    [Export]
    private VBoxContainer buttonHolder;
    private Dictionary<Button, bool> buttonsInformation = [];


    public override void _Ready()
    {
        Toggled += toggle;
        order.Pressed += toggleOrder;
        SkinManager.Instance.Loaded += UpdateSkin;

        foreach (var node in buttonHolder.GetChildren())
        {
            var button = (Button)node;
            if (!buttonsInformation.TryAdd(button, true))
            {
                Logger.Error("Tried adding the same reference of button twice!");
            }
            checkButtonStateColor(button);
            button.Pressed += () => selectFilter(button);
            button.Pressed += () => checkButtonStateColor(button);
        }

        UpdateSkin(SkinManager.Instance.Skin);
    }

    private void checkButtonStateColor(Button button)
    {
        buttonsInformation.TryGetValue(button, out bool state);
        if (state)
        {
            button.Modulate = new Color(1f, 1f, 1f);
        }
        else
        {
            button.Modulate = new Color(1f, 1f, 1f, 0.3f);
        }
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            Rect2 rect = new(panel.GlobalPosition, panel.Size);
            Rect2 rect2 = new(GlobalPosition, Size);

            if (!rect.HasPoint(mouseButton.Position) && !rect2.HasPoint(mouseButton.Position))
            {
                ButtonPressed = false;
            }
        }
    }

    public void UpdateSkin(SkinProfile skin)
    {
        Icon = order.Icon;
    }

    private void toggle(bool toggled)
    {
        panel.Visible = toggled;
    }

    private void selectFilter(Button button)
    {
        buttonsInformation.TryGetValue(button, out bool isActiveState);
        Enum.TryParse<MapList.FilterType>(button.Name, true, out var result);
        if (!isActiveState)
        {
            addFilterItem(result);
            button.TextureFilter = TextureFilterEnum.Max;
        }
        else
        {
            removeFilteritem(result);
            button.TextureFilter = TextureFilterEnum.Linear;
        }

        buttonsInformation[button] = !isActiveState;
    }

    private void addFilterItem(MapList.FilterType filter)
    {
        Logger.Log($"Added filter {filter.ToString()}");
        MapList.Instance.Filters.Add(filter);
    }

    private void removeFilteritem(MapList.FilterType filter)
    {
        Logger.Log($"Removed filter {filter.ToString()}");
        MapList.Instance.Filters.Remove(filter);
    }

    private void toggleOrder()
    {
        var skin = SkinManager.Instance.Skin;

        MapList.Instance.Ascending.Value = !MapList.Instance.Ascending.Value;
        orderLabel.Text = MapList.Instance.Ascending.Value ? "Ascending" : "Descending";
        order.Icon = MapList.Instance.Ascending.Value ? skin.SortAscendButtonImage : skin.SortButtonImage;

        Icon = order.Icon;
    }
}
