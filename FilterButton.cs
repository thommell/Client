using System;
using System.Collections.Generic;
using Godot;


public partial class FilterButton : Button, ISkinnable
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

    private Button previousButton;

    public override void _Ready()
    {
        Toggled += toggle;
        order.Pressed += toggleOrder;
        SkinManager.Instance.Loaded += UpdateSkin;

        previousButton = buttonHolder.GetNode<Button>("Alphabetical");

        foreach (var node in buttonHolder.GetChildren())
        {
            var button = (Button)node;
            if (!buttonsInformation.TryAdd(button, false))
            {
                Logger.Error("Tried adding the same reference of button twice!");
            }
            button.Pressed += () => selectFilter(button);
        }

        UpdateSkin(SkinManager.Instance.Skin);
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
        order.Icon = MapList.Instance.Ascending.Value ? skin.SortAscendButtonImage : skin.SortButtonImage;
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
        }
        else
        {
            removeFilteritem(result);
        }

        buttonsInformation[button] = !isActiveState;

        return;
        if (MapList.Instance.Filters.Count != 0)
        {
            MapList.Instance.Filters.Clear();
            Logger.Log("Resetting filters.");
            return;
        }
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
