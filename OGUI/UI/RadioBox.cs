﻿using System;
using System.Collections.Generic;
using DEngine.Core;
using OGUI.Core;

namespace OGUI.UI
{
    #region RadioGroup Helper Classes
    /// <summary>
    /// This is the argument sent as part of a RadioBox.RadioToggled event.
    /// </summary>
    public class RadioToggledEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a RadioToggledEventArgs object with the specified item radio index.
        /// </summary>
        /// <param name="index"></param>
        public RadioToggledEventArgs(int index)
        {
            Index = index;
        }

        /// <summary>
        /// The index of the toggled radio.
        /// </summary>
        public int Index { get; private set; }

    }

    /// <summary>
    /// Contains the label and tooltip text for each RadioItem that will be added
    /// to a Listbox.
    /// </summary>
    public class RadioItemData
    {
        /// <summary>
        /// Construct a ListItemData instance given the label and an optional tooltip.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toolTip"></param>
        public RadioItemData(string label, string toolTip = null)
        {
            this.Label = label;
            this.TooltipText = toolTip;
        }

        /// <summary>
        /// The label of this radio item.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The optional tooltip text for this radio item.
        /// </summary>
        public string TooltipText { get; set; }
    }
    #endregion

    #region RadioGroupTemplate
    /// <summary>
    /// Used to contructs a RadioGroup object.
    /// </summary>
    public class RadioGroupTemplate : ControlTemplate
    {
 
        /// <summary>
        /// Default constructor initializes properties to their defaults.
        /// </summary>
        public RadioGroupTemplate()
        {
            Items = new List<RadioItemData>();
            LabelAlignment = HorizontalAlignment.Left;
            InitialSelectedIndex = 0;
            CanHaveKeyboardFocus = false;
            HilightRadioMouseOver = false;
            HasFrameBorder = true;
            RadioOnLeft = true;
        }
 

        /// <summary>
        /// The list of RadioItemData elements that will be included in the radio group.  Defaults
        /// to an empty list.
        /// </summary>
        public List<RadioItemData> Items { get; set; }

        /// <summary>
        /// The horizontal alignment of the radio labels.  Defaults to left.
        /// </summary>
        public HorizontalAlignment LabelAlignment { get; set; }

        /// <summary>
        /// Which radio index will be selected initially.  Defaults to 0.
        /// </summary>
        public int InitialSelectedIndex { get; set; }

        /// <summary>
        /// Specifies if this control can receive the keyboard focus.  Defaults to false.
        /// </summary>
        public bool CanHaveKeyboardFocus { get; set; }

        /// <summary>
        /// Specifies if the radio item is drawn in hilighted colors when under the mouse pointer.
        /// Defaults to false.
        /// </summary>
        public bool HilightRadioMouseOver { get; set; }

        /// <summary>
        /// Use this to manually size the RadioGroup.  If this is empty (the default), then the
        /// RadioGroup will autosize.
        /// </summary>
        public Size AutoSizeOverride { get; set; }

        /// <summary>
        /// If true, a frame will be drawn around the RadioGroup.
        /// If autosizing, the required space for the frame element will be added.
        /// Defaults to true.
        /// </summary>
        public bool HasFrameBorder { get; set; }
 

        /// <summary>
        /// Set to true if the radio element will be drawn to the left of the label.  Otherwise
        /// the label will be drawn left of the radio element.  Defaults to true.
        /// </summary>
        public bool RadioOnLeft { get; set; }

        /// <summary>
        /// The title string if there is a frame (if there is no frame, this doesn't affect anything), defaults to ""
        /// </summary>
        public string Title { get; set; }

 
        /// <summary>
        /// Calculates the RadioGroup size based on the properties of this template.
        /// </summary>
        /// <returns></returns>
        public override Size CalculateSize()
        {
            if (AutoSizeOverride.Width > 0 && AutoSizeOverride.Height > 0)
            {
                return AutoSizeOverride;
            }

            int width = 0;
            foreach (RadioItemData i in Items)
            {
                if (i.Label == null)
                    i.Label = "";

                if (Canvas.TextLength(i.Label) > width)
                    width = Canvas.TextLength(i.Label);
            }

            width += 2; // room for the radio element

            int height = Items.Count;

            if (HasFrameBorder)
            {
                width += 2;
                height += 2;
            }

            return new Size(width, height);
        }
    }
    #endregion

    #region RadioGroup
    /// <summary>
    /// Represents a group (list) of radio boxes.  Only one radio can be selected (toggled) at
    /// a time.
    /// </summary>
    public class RadioGroup : Control
    {
        #region Events
        /// <summary>
        /// Raised when a radio box has been toggled (selected) by user input.  The
        /// RadioToggledEventArgs contains the index number of the selected radio.
        /// </summary>
        public event EventHandler<RadioToggledEventArgs> RadioToggled;
        #endregion
        #region Constructors
        /// <summary>
        /// Construct a RadioGroup from the specified templated.
        /// </summary>
        /// <param name="template"></param>
        public RadioGroup(RadioGroupTemplate template)
            : base(template)
        {
            HasFrame = template.HasFrameBorder;

            if (Size.Width < 3 || Size.Height < 3)
            {
                HasFrame = false;
            }

            HilightWhenMouseOver = false;

            HilightRadioMouseOver = template.HilightRadioMouseOver;
            CanHaveKeyboardFocus = template.CanHaveKeyboardFocus;
            LabelAlignment = template.LabelAlignment;
            Items = template.Items;
            mouseOverIndex = -1;
            RadioOnLeft = template.RadioOnLeft;
            Title = template.Title;

            CurrentSelected = template.InitialSelectedIndex;

            if (CurrentSelected < 0 || CurrentSelected >= Items.Count)
            {
                CurrentSelected = 0;
            }

            CalcMetrics(template);

        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The alignment of the radio labels.
        /// </summary>
        public HorizontalAlignment LabelAlignment { get; set; }

        /// <summary>
        /// Set to true if the radio element will be drawn to the left of the label.  Otherwise
        /// the label will be drawn left of the radio element.
        /// </summary>
        public bool RadioOnLeft { get; set; }

        /// <summary>
        /// The currently selected radio index.
        /// </summary>
        public int CurrentSelected { get; protected set; }

        /// <summary>
        /// Specifies if the radio item is drawn in hilighted colors when under the mouse pointer.
        /// </summary>
        public bool HilightRadioMouseOver { get; protected set; }

        /// <summary>
        /// The title string.
        /// </summary>
        public string Title { get; private set; }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Draws all of the items.
        /// </summary>
        protected void DrawItems()
        {
            for (int i = 0; i < numberItemsDisplayed; i++)
            {
                DrawItem(i);
            }
        }

        /// <summary>
        /// Draws the specified item.
        /// </summary>
        /// <param name="index"></param>
        protected void DrawItem(int index)
        {
            RadioItemData item = Items[index];
            Pigment pigment;

            if (mouseOverIndex == index && HilightRadioMouseOver)
            {
                pigment = Pigments[PigmentType.ViewHilight];
            }
            else
            {
                pigment = DetermineMainPigment();
            }

            if (labelRect.Size.Width > 0 &&
                !string.IsNullOrEmpty(item.Label))
            {
                Canvas.PrintStringAligned(labelRect.TopLeft.X,
                    labelRect.TopLeft.Y + index,
                    item.Label,
                    LabelAlignment,
                    labelRect.Size.Width,
                    pigment);
            }
            char rc;

            if (CurrentSelected == index)
            {
                rc = (char)10;
            }
            else
            {
                rc = (char)9;
            }

            Canvas.PrintChar(radioRect.TopLeft.X,
                radioRect.TopLeft.Y + index,
                rc, 
                pigment);
        }

        /// <summary>
        /// Returns the index of the item at the given position, or -1 if there is not item
        /// at that position.  The position is given in local space coordinates.
        /// </summary>
        /// <param name="lPos"></param>
        /// <returns></returns>
        protected int GetItemAt(Point lPos)
        {
            int index = -1;

            if (itemsRect.Contains(lPos))
            {
                index = lPos.Y - itemsRect.Top;
            }
            if (index < 0 || index >= Items.Count)
            {
                index = -1;
            }
            return index;
        }

        protected override void DrawFrame(Pigment pigment = null) {
            if (this.Size.Width > 2 && this.Size.Height > 2)
                Canvas.PrintFrame(string.IsNullOrEmpty(Title) ? null : Title, pigment);
        }

        /// <summary>
        /// Draws the radio items.  Override to add custom drawing code.
        /// </summary>
        protected override void Redraw()
        {
            base.Redraw();

            DrawItems();
        }

        /// <summary>
        /// Base method detects if the mouse pointer is currently over a radio item and
        /// sets the state accordingly.  Override to add custom handling.
        /// </summary>
        /// <param name="mouseData"></param>
        protected internal override void OnMouseMoved(MouseData mouseData)
        {
            base.OnMouseMoved(mouseData);

            Point lPos = ScreenToLocal(mouseData.Position);

            mouseOverIndex = GetItemAt(lPos);

            if (mouseOverIndex != -1)
            {
                TooltipText = Items[mouseOverIndex].TooltipText;
            }
            else
            {
                TooltipText = null;
            }
        }

        /// <summary>
        /// Base method detects if a radio item was selected, and calls OnItemSelected if this
        /// is the case.  Override to add custom handling.
        /// </summary>
        /// <param name="mouseData"></param>
        protected internal override void OnMouseButtonDown(MouseData mouseData)
        {
            base.OnMouseButtonDown(mouseData);

            if (mouseOverIndex != -1)
            {
                OnItemSelected(mouseOverIndex);
            }
        }

        /// <summary>
        /// Triggers the appropriate event, and removes this menu from the parent window.  Override
        /// to add custom handling.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void OnItemSelected(int index)
        {
            CurrentSelected = index;
            if (RadioToggled != null)
            {
                RadioToggled(this, new RadioToggledEventArgs(index));
            }
        }
        #endregion

        #region Private
        private List<RadioItemData> Items;
        private int mouseOverIndex;
        private Rect itemsRect;
        private int numberItemsDisplayed;

        private Rect radioRect;
        private Rect labelRect;

        private void CalcMetrics(RadioGroupTemplate template)
        {
            itemsRect = this.LocalRect;
            if (HasFrame)
            {
                itemsRect = Rect.Inflate(itemsRect, -1, -1);
            }

            int delta = itemsRect.Size.Height - Items.Count;

            numberItemsDisplayed = Items.Count;

            if (delta < 0)
            {
                numberItemsDisplayed += delta;
            }

            if (RadioOnLeft)
            {
                radioRect = new Rect(itemsRect.TopLeft, new Size(1, 1));
                labelRect = new Rect(radioRect.TopRight.Shift(2,0), 
                    itemsRect.TopRight);
            }
            else
            {
                radioRect = new Rect(itemsRect.TopRight, new Size(1, 1));
                labelRect = new Rect(itemsRect.TopLeft, 
                    radioRect.BottomLeft.Shift(-2,0));
            }
        }
        #endregion
    }
    #endregion
}
