using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Extension.Checker;
using Extension.Manipulator;

namespace Extension.Controls {
  public enum DoubleListBoxPanelStyle {
    Combo,
    Bits
  }

  public class DoubleListBoxPanel : Panel { //either as ComboBox or as Bits
    private ReadOnlyListBox leftListBox = new ReadOnlyListBox(); //represents the actual value, not the possible values
    private ReadOnlyListBox rightListBox = new ReadOnlyListBox(); //represents the text strings represented the possible values
    private List<string> valueList = new List<string>(); //represents the possible values (always hidden)

    private DoubleListBoxPanelStyle listBoxStyle = DoubleListBoxPanelStyle.Combo;
    public DoubleListBoxPanelStyle ListBoxStyle { get { return listBoxStyle; } set { listBoxStyle = value; } }

    private bool disableFocusCue = false;
    public bool DisableFocusCue { get { return disableFocusCue; } set { disableFocusCue = value; } }

    protected override bool ShowFocusCues { get { return disableFocusCue ? false : base.ShowFocusCues; } }

    private string dataType = "uint8";
    public string DataType { 
      get { return dataType; }
      set {
				if (Extension.Checker.Text.IsRecognizedDataType(value))
          this.dataType = value.ToLower(); //don't forget to put lower case for this
      }
    }

    public DoubleListBoxPanel() { //must be initialized with a "datatype"
      //left list box
      leftListBox.BorderStyle = BorderStyle.None;
      leftListBox.Location = new Point(0, 0);
      leftListBox.Font = new Font(leftListBox.Font, FontStyle.Bold);
      leftListBox.ForeColor = Color.Blue;
      leftListBox.Width = 12; //TODO subjected to font size      
      leftListBox.ReadOnly = true;
      leftListBox.DisableFocusCue = true;
      leftListBox.TabStop = false; //such that the readonly becomes complete      
      leftListBox.Show();
      this.Controls.Add(leftListBox);

      //right list box
      rightListBox.BorderStyle = BorderStyle.None;
      rightListBox.Location = new Point(leftListBox.Width, 0);
      rightListBox.ForeColor = Color.DimGray;
      rightListBox.ReadOnly = true;
      rightListBox.DisableFocusCue = true;
      rightListBox.TabStop = false; //such that the readonly becomes complete
      rightListBox.Show();
      this.Controls.Add(rightListBox);

      //this
      this.Size = new Size(rightListBox.Width + leftListBox.Width, rightListBox.Height);
    }


    public void SetLists(List<string> stringList, List<string> valueList) {
      leftListBox.Items.Clear(); //clear comes first
      rightListBox.Items.Clear();
      if (stringList == null || stringList.Count <= 0 || valueList == null || valueList.Count <= 0 || stringList.Count != valueList.Count)
        return;
      rightListBox.Items.AddRange(stringList.ToArray());
      SizeF stringSize = new SizeF(0, 0);
      for (int i = 0; i < stringList.Count; ++i) {
        SizeF textSizeF = rightListBox.CreateGraphics().MeasureString(stringList[i], rightListBox.Font); //create graphics such that we won't retain the Graphics object
        if (textSizeF.Height > stringSize.Height)
          stringSize.Height = textSizeF.Height;
        if (textSizeF.Width > stringSize.Width)
          stringSize.Width = textSizeF.Width;
      }
      rightListBox.Height = rightListBox.PreferredHeight;      
      leftListBox.Height = rightListBox.Height;
      rightListBox.Width = (int)(stringSize.Width + 4 + 0.5); //0.5 is for rounding, 4 for margin
      this.Size = new Size(rightListBox.Width + leftListBox.Width, rightListBox.Height);
      this.valueList = valueList;
    }

    private bool invalidDisplayValue = false;
    public bool InvalidDisplayValue { get { return invalidDisplayValue; } } //TODO can be used to make the reading abnormal

    //\u1F311 new moon symbol, \u2b24 black large circle, \u25cf black circle, \u25ef large circle, \u2666 diamond, 
    //\u05d0 = aleph, \u220E (square), \u23fa circle, , \u23f9 square, \u25b2 triangle
    //display value is capable of display something with "|"
    private string displayValue;
    public string DisplayValue { 
      get { return displayValue; }
      set {
        displayValue = value;
        leftListBox.Items.Clear();
        invalidDisplayValue = true; //until proven otherwise
        if (displayValue == null) {
          leftListBox.Width = 12; //TODO subjected to font size
          rightListBox.Location = new Point(leftListBox.Width, 0);
          this.Size = new Size(rightListBox.Width + leftListBox.Width, rightListBox.Height);
          return;
        }
        string[] testSplit = displayValue.Split(new char[] { '|' });
        string originalValue = testSplit != null && testSplit.Length > 1 ? testSplit[0] : displayValue;
        string addDisplayVal = testSplit != null && testSplit.Length > 1 ? testSplit[1] : null;
        leftListBox.Width = addDisplayVal == null ? 12 : 24; //TODO subjected to font size
        rightListBox.Location = new Point(leftListBox.Width, 0);
        this.Size = new Size(rightListBox.Width + leftListBox.Width, rightListBox.Height);
        if (listBoxStyle == DoubleListBoxPanelStyle.Combo) {
          for (int i = 0; i < valueList.Count; ++i) { //value list always there            
						if (Extension.Checker.Text.StringsHaveTheSameValue(originalValue, valueList[i], dataType)) { //found
              invalidDisplayValue = false;
							leftListBox.Items.Add(addDisplayVal == null ? "\u2666" : (Extension.Checker.Text.StringsHaveTheSameValue(addDisplayVal, valueList[i], dataType) ? "\u2666|\u2666" : "\u2666| "));
              if (addDisplayVal == null)
                break;
            } else
							leftListBox.Items.Add(addDisplayVal == null ? " " : (Extension.Checker.Text.StringsHaveTheSameValue(addDisplayVal, valueList[i], dataType) ? "  |" + ('\u2666').ToString() : "  |  "));
          }
        } else if (listBoxStyle == DoubleListBoxPanelStyle.Bits) { //for bits, the value must necessarily be able to be converted into integer, otherwise it fails!
          try {
            if (string.IsNullOrWhiteSpace(originalValue)) { //if the original value is clearly bad, it can be skipped
              if (addDisplayVal != null) {
                for (int i = 0; i < Math.Min(valueList.Count, 32); ++i) { //maximum option which can be provided here is 32...                 
									uint addUIntVal = Extension.Checker.Text.IsHex(addDisplayVal) ? Data.HexString0xToUint(addDisplayVal) : Convert.ToUInt32(addDisplayVal);
                  string val = (addUIntVal & (1 << i)) > 0 ? " |1" : " |0";
                  leftListBox.Items.Add(val);
                }
              }
            } else { //else, process the original value first
							uint uintVal = Extension.Checker.Text.IsHex(originalValue) ? Data.HexString0xToUint(originalValue) : Convert.ToUInt32(originalValue);
              for (int i = 0; i < Math.Min(valueList.Count, 32); ++i) { //maximum option which can be provided here is 32...
                string val = (uintVal & (1 << i)) > 0 ? "1" : "0";
                if (addDisplayVal != null) {
                  if (string.IsNullOrWhiteSpace(addDisplayVal)) //is white space
                    val += "|";
                  else { //is not whitespace
										uint addUIntVal = Extension.Checker.Text.IsHex(addDisplayVal) ? Data.HexString0xToUint(addDisplayVal) : Convert.ToUInt32(addDisplayVal);
                    val += (addUIntVal & (1 << i)) > 0 ? "|1" : "|0";
                  }
                }
                leftListBox.Items.Add(val);
              }
            }
            invalidDisplayValue = false; //proven to be correct
          } catch {
            //is not handled, but it means that the conversion fails somewhere
          }
        }
      }
    }



  }
}
