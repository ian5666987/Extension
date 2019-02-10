using System.Collections.Generic;

using Extension.Reader;

namespace Extension.Developer
{
  public class GenericDataStream : FileDirText
  {
		public static List<GenericData> CreateGenericData(string folderpath, string extension, bool includeSubfolder = true,			
			bool isTrimmed = false, bool markInclusionExclusion = false, char[] marks = null, bool stripmark = false, char[] strippedmarks = null) {
			try {
				List<string> files = ReadAllFiles(folderpath, extension, includeSubfolder);
				List<GenericData> genericDataList = new List<GenericData>();
				foreach (string file in files) { //will the file be in full path or shortpath?
					List<GenericData> tempGenData = CreateGenericData(file, isTrimmed, markInclusionExclusion, marks,
						stripmark, strippedmarks);
					if (tempGenData == null || tempGenData.Count <= 0)
						continue;
					if (genericDataList.Count <= 0) { //if the generic data list is still empty
						genericDataList.AddRange(tempGenData.ToArray());
						continue;
					}
					foreach (GenericData genDat in tempGenData) { //if the generic data list is not empty, beware the need of merging
						GenericData oldData = genericDataList.Find(x => x.DataName == genDat.DataName);
						if (oldData == null) { //new data
							genericDataList.Add(genDat);
							continue;
						}
						oldData.CombineAndOverride(genDat); //old data
					}
				}
				return genericDataList;
			} catch {
				return null;
			}
		}

    //List<string> validLines = GetAllValidLines(filepath, isTrimmed: true, markInclusionExclusion: false, marks: new char[] { '#' }, stripmark: false, strippedmarks: new char[] { '!' }); //all lines which are not empty or whitespace
    public static List<GenericData> CreateGenericData(string filepath, bool isTrimmed = false, bool markInclusionExclusion = false, char[] marks = null, bool stripmark = false, char[] strippedmarks = null) {
      List<string> validLines = GetAllValidLines(filepath, isTrimmed, markInclusionExclusion, marks, stripmark, strippedmarks); //all lines which are not empty or whitespace
      if (validLines == null || validLines.Count <= 0)
        return null; //fails to create
      List<GenericData> genericDataList = new List<GenericData>(); //TODO needs to be corrected! There are some mistakes here!
      for (int i = 0; i < validLines.Count; ++i) { //There are two ways to read this item, that is whether after the first word '.' is contained or not!
        //string[] properties = ReadUntilMeet(validLines[i], '=').Contains(".") ? SplitBeforeAndAfterMeet(validLines[i], '=', true, '[', ']', isInclusive: false, isTrimmed: true) : validLines[i].Split(new char[] { ';', '=' }); //either two words or multi-words
        string[] properties = null;
        if (ReadUntilMeet(validLines[i], '=').Contains("."))
          properties = SplitBeforeAndAfterMeet(validLines[i], '=', true, '[', ']', isInclusive: false, isTrimmed: true);
        else
          properties = validLines[i].Split(new char[] { ';', '=' });
        genericDataList.Add(new GenericData(properties)); //creates generic data
      }
      return genericDataList.Count > 0 ? genericDataList : null;
    }
  }
}


//Scenario:
// given a property file, reads it, interprets it
// put the interpretation to the right data structure (or class), called with generic name: GenericData
// then, we will have data structure (or class) with such name

// Methods needed:
// (1) ReadFile, get all valid lines -> can be achieved by other reader
// (2) put the interpretation to a class: GenericData -> needs to define such class, and the rests of the reader can be built on top of this class (display classes)

//Such classes should be made so that it is compatible to read them by
// another class, which is to fit with the page manager

//on top of that, some property files can be read in terms of:
// 1) packet: whatever is in that property file belong to the same packet, sent together
// 2) merge: same as info, except that this property file types adds something to what isn't there... (combo, data value, bits, infos/tooltip)

//Example:
// property reader will be able to read a [packet] like:
//
//! XData_Header
//! XData_Get_Id
//! OBDH_Address
//! XDATA_OBDH_MET_OFFSET             =value=1400;type=uint16;policy=auto;
//! XData_Length

//! OBDH_Time                         =type=uint32;min=1735689635;max=1956528035;unit=UTC;
//! OBDH_Uptime_Total                 =type=uint32;unit=sec;
//! OBDH_StorePeriod                  =type=uint16;min=20;unit=sec;
//! OBDH_Uptime_Session               =type=uint32;unit=sec;policy=read;

//! OBDH_Chip_Oscillator              =type=uint8;policy=read;
//! OBDH_Chip_SystemFreq              =type=uint32;unit=Hz;policy=read;
//! OBDH_Chip_TotalResets             =type=uint32;
//! OBDH_Chip_ResetSource             =type=uint8;policy=read;
//! OBDH_Chip_SPI0_failures           =type=uint16;max=0;
//! OBDH_Chip_Temperature             =type=float;unit=deg C;min=-55;max=125;policy=read;

// or to read the packet like this to [merge]: additional (value, combo, bits)
//OBDH_Address                    =value=0x0a;type=uint8;policy=auto;

//OBDH_Chip_Oscillator.combo      =[external=0, internal=1]
//OBDH_Chip_ResetSource.bits      =[hw_pin=0, power_on=1, missing_clock=2, watchdog=3, software_force=4, comparator=5, convert_start_0=6]

//OBDH_St_PwrsHk_mode.combo       =[disabled=0, repeated=3]
//OBDH_St_AdcsHk_mode.combo       =[disabled=0, repeated=3]

// or to read the packet like this to [merge]:info

//Sched_00_Time   =TAI time for execution of scheduler entry queued at 0
//Sched_00_System =subsystem to receive command of scheduler entry queued at 0
//Sched_00_Offset =memory address to transfer data of scheduler entry queued at 0
//Sched_00_Length =number of bytes to copy by scheduler entry queued at 0

//Do something about this reader!
//After the reader is made, the page handler can be made
//There might be some more advance reader involved

