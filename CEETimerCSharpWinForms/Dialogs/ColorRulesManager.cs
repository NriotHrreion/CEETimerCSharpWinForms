﻿using CEETimerCSharpWinForms.Forms;
using CEETimerCSharpWinForms.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CEETimerCSharpWinForms.Dialogs
{
    public partial class ColorRulesManager : Form
    {
        public List<PairItems<PairItems<int, TimeSpan>, PairItems<Color, Color>>> ColorRules { get; set; }

        private bool IsEditMode;
        private bool RulesChanged;
        private bool IsFormLoading;
        private ListView.ListViewItemCollection GetAllItems() => ListViewMain.Items;

        public ColorRulesManager()
        {
            IsFormLoading = true;
            InitializeComponent();
            Shown += (sender, e) => IsFormLoading = false;
            TopMost = FormMain.IsUniTopMost;
            RulesChanged = false;
        }

        private void ColorRulesManager_Load(object sender, EventArgs e)
        {
            if (ColorRules != null || ColorRules.Count == 0)
            {
                ListViewMain.Items.Clear();

                foreach (var Rule in ColorRules)
                {
                    var Part1 = Rule.Item1;
                    var Part2 = Rule.Item2;
                    AddListViewItem(Part1.Item1, ColorRulesHelper.GetExamTickText(Part1.Item2 - new TimeSpan(0, 0, 0, 1)), Part2.Item1, Part2.Item2);
                }
            }
        }

        private void ContextAdd_Click(object sender, EventArgs e)
        {
            ColorRuleDialog _ColorRuleDialog = new();

            if (_ColorRuleDialog.ShowDialog() == DialogResult.OK)
            {
                AddListViewItem(_ColorRuleDialog.RuleType, _ColorRuleDialog.ExamTick, _ColorRuleDialog.Fore, _ColorRuleDialog.Back);
            }
        }

        private void ContextEdit_Click(object sender, EventArgs e)
        {
            EditColorRule(ListViewMain.SelectedItems[0]);
        }

        private void ListViewMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && ListViewMain.SelectedItems.Count != 0)
            {
                ContextDelete_Click(sender, e);
            }
        }

        private void ContextDelete_Click(object sender, EventArgs e)
        {
            if (MessageX.Popup("确认删除所选规则吗？此操作将不可撤销！", MessageLevel.Warning, this, Buttons: MessageBoxExButtons.YesNo) == DialogResult.Yes)
            {
                foreach (ListViewItem Item in ListViewMain.SelectedItems)
                {
                    ListViewMain.Items.Remove(Item);
                }
            }
        }

        private void ContextMenuMain_Opening(object sender, CancelEventArgs e)
        {
            ContextDelete.Enabled = ListViewMain.SelectedItems.Count != 0;
            ContextEdit.Enabled = ListViewMain.SelectedItems.Count == 1;
        }

        private void ListViewMain_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }

        private void ListViewMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ListViewMain.HitTest(e.X, e.Y).Item != null && ListViewMain.SelectedItems.Count == 1)
            {
                EditColorRule(ListViewMain.GetItemAt(e.X, e.Y));
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            ColorRules = [];

            foreach (ListViewItem Item in GetAllItems())
            {
                ColorRules.Add(new(new(ColorRulesHelper.GetRuleTypeIndex(Item.SubItems[0].Text), ColorRulesHelper.GetExamTick(Item.SubItems[1].Text)), new(ColorHelper.GetColor(Item.SubItems[2].Text), ColorHelper.GetColor(Item.SubItems[3].Text))));
            }

            RulesChanged = false;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ColorRulesManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RulesChanged)
            {
                switch (MessageX.Popup("检测到颜色规则已更改，是否保存？", MessageLevel.Warning, this, Buttons: MessageBoxExButtons.YesNo))
                {
                    case DialogResult.Yes:
                        e.Cancel = true;
                        ButtonOK_Click(sender, e);
                        break;
                    case DialogResult.None:
                        e.Cancel = true;
                        break;
                    default:
                        RulesChanged = false;
                        Close();
                        break;
                }
            }
        }

        private void AddListViewItem(int RuleTypeIndex, string ExamTick, Color Fore, Color Back, ListViewItem Item = null)
        {
            RulesChanged = !IsFormLoading;
            var RuleTypeText = ColorRulesHelper.GetRuleTypeText(RuleTypeIndex);
            var _Fore = Fore.ToRgb();
            var _Back = Back.ToRgb();

            if (!IsEditMode)
            {
                var Duplicate = GetDuplicate(RuleTypeText, ExamTick);

                if (Duplicate != null)
                {
                    if (MessageX.Popup("检测到即将添加的规则与现有的重复，是否覆盖？", MessageLevel.Warning, this, Buttons: MessageBoxExButtons.YesNo) == DialogResult.Yes)
                    {
                        ModifyOrOverrideItem(Duplicate);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (Item != null)
            {
                ModifyOrOverrideItem(Item);
                return;
            }

            ListViewMain.Items.Add(new ListViewItem([$"{RuleTypeText}", $"{ExamTick}", $"{_Fore}", $"{_Back}"]));
            AdjustColumnWidth();

            void ModifyOrOverrideItem(ListViewItem Item)
            {
                Item.SubItems[0].Text = $"{RuleTypeText}";
                Item.SubItems[1].Text = $"{ExamTick}";
                Item.SubItems[2].Text = $"{_Fore}";
                Item.SubItems[3].Text = $"{_Back}";
                AdjustColumnWidth();
            }
        }

        private ListViewItem GetDuplicate(string Column1Text, string Column2Text)
        {
            foreach (ListViewItem Item in GetAllItems())
            {
                if (Item.SubItems[0].Text == Column1Text && Item.SubItems[1].Text == Column2Text)
                {
                    return Item;
                }
            }

            return null;
        }

        private void EditColorRule(ListViewItem Item)
        {
            IsEditMode = true;

            ColorRuleDialog _ColorRuleDialog = new()
            {
                RuleType = ColorRulesHelper.GetRuleTypeIndex(Item.SubItems[0].Text),
                ExamTick = Item.SubItems[1].Text,
                Fore = ColorHelper.TryParseRGB(Item.SubItems[2].Text, out Color color1) ? color1 : Color.White,
                Back = ColorHelper.TryParseRGB(Item.SubItems[3].Text, out Color color2) ? color2 : Color.White
            };

            if (_ColorRuleDialog.ShowDialog() == DialogResult.OK)
            {
                AddListViewItem(_ColorRuleDialog.RuleType, _ColorRuleDialog.ExamTick, _ColorRuleDialog.Fore, _ColorRuleDialog.Back, Item);
            }

            IsEditMode = false;
        }

        private void AdjustColumnWidth()
        {
            foreach (ColumnHeader column in ListViewMain.Columns)
            {
                column.Width = -2;
            }

            SortItems();
        }

        private void SortItems()
        {
            List<ListViewItem> Items = ListViewMain.Items.Cast<ListViewItem>().ToList();
            Items.Sort(new ListViewItemComparer());
            ListViewMain.BeginUpdate();
            ListViewMain.Items.Clear();
            ListViewMain.Items.AddRange([.. Items]);
            ListViewMain.EndUpdate();
        }
    }
}
