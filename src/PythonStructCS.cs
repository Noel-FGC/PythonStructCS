using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PythonStructCS { 
  internal class TypeSpecifier {
    public int size = 0;
    public string type = "Unset";
  }
  
  public static class PyStruct {
    static readonly char[] ValidEndians = ['@', '<', '>', '=', '!'];
    public static byte[] pack(string format, List<object> data) {
      
      if (format.Length == 0 && data.Count == 0) {
        return new byte[0];
      }

      var returnArray = new byte[calcSize(format)];

      var types = getFormatTypeList(format);
      List<TypeSpecifier> neededTypes = [];
      foreach (var valueType in types) {
        if (valueType.type != "Pad") {
          neededTypes.Add(valueType);
        }
      }

      if (data.Count != neededTypes.Count) {
        throw new ArgumentException("Received Incorrect Amount Of Items For Packing (Expected " + neededTypes.Count + ", Received " + data.Count + ")");
      }

      char Endian = '<';
      if(ValidEndians.Contains(format[0])) {
        Endian = format[0];
        if (((char[])['@', '=']).Contains(Endian)) {
          Endian = '<';
        } else if (Endian == '!') {
          Endian = '>';
        }
      } else {
        Debug.WriteLine("No Endian Provided, Defaulting To '<' (Little)");
      }

      var memoryStream = new MemoryStream();
      var writer = new BinaryWriter(memoryStream);
      foreach (var valueType in types) {
        switch (valueType.type) {
          case "Pad": {
            writer.Write((byte)0);
          } break;
          case "Char": {
             var tmp = BitConverter.GetBytes(Convert.ToChar(data[0]));
             if (Endian == '>') {
               Array.Reverse(tmp);
             }
             writer.Write(tmp);
             data.RemoveAt(0);
           } break;
          case "SByte": {
            writer.Write(Convert.ToSByte(data[0]));
            data.RemoveAt(0);
          } break;
          case "Byte": {
            writer.Write(Convert.ToByte(data[0]));
            data.RemoveAt(0);
          } break;
          case "Boolean": {
            var tmp = BitConverter.GetBytes(Convert.ToBoolean(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "Int16": {
            var tmp = BitConverter.GetBytes(Convert.ToInt16(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "UInt16": {
            var tmp = BitConverter.GetBytes(Convert.ToUInt16(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "Int32": {
            var tmp = BitConverter.GetBytes(Convert.ToInt32(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "UInt32": {
            var tmp = BitConverter.GetBytes(Convert.ToUInt32(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "Int64": {
            var tmp = BitConverter.GetBytes(Convert.ToInt64(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "UInt64": {
            var tmp = BitConverter.GetBytes(Convert.ToUInt64(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "Half": {
            var tmp = BitConverter.GetBytes(Convert.ToSingle(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0); 
          } break;
          case "Single": {
            var tmp = BitConverter.GetBytes(Convert.ToSingle(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "Double": {
            var tmp = BitConverter.GetBytes(Convert.ToDouble(data[0]));
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            writer.Write(tmp);
            data.RemoveAt(0);
          } break;
          case "String": {
            string tempString = Convert.ToString(data[0]);
            if (valueType.size < tempString.Length) {
              tempString = tempString.Substring(0, valueType.size);
            }
            byte[] stringBytes = Encoding.UTF8.GetBytes(tempString);
            writer.Write(stringBytes);
            if (valueType.size > tempString.Length) {
              int tmp = valueType.size - tempString.Length; 
              for (int i = 0;i < tmp;i++) {
                writer.Write((byte)0);
              }
            }
            data.RemoveAt(0);
          } break;
        }
      }

      returnArray = memoryStream.ToArray();
      return returnArray;
    }



    public static List<object> unpack(string format, byte[] data) {
      var memoryStream = new MemoryStream(data);
      var reader = new BinaryReader(memoryStream);
      char Endian = '<';
      List<object> returnList = [];
      var types = getFormatTypeList(format);

      if (format.Length == 0) {
        return returnList;
      }

      if(ValidEndians.Contains(format[0])) {
        Endian = format[0];
        if (((char[])['@', '=']).Contains(Endian)) {
          Endian = '<';
        } else if (Endian == '!') {
          Endian = '>';
        }
      } else {
        Debug.WriteLine("No Endian Provided, Defaulting To '<' (Little)");
      }



      foreach (var valueType in types) {
        object val = "";
        switch (valueType.type) {
          case "Pad": {
            Debug.WriteLine("Skipping Pad Byte");
            var stupid = reader.ReadByte();
            continue;
          }
          case "Char": { // 1 byte
            val = reader.ReadChar();
          } break;
          case "SByte": { // 1 byte
            val = reader.ReadSByte();
          } break;
          case "Byte": { // 1 byte
            val = reader.ReadByte();
          } break;
          case "Boolean": { // 1 byte
            val = reader.ReadBoolean();
          } break;
          case "Int16": { // 2 bytes
            var tmp = reader.ReadBytes(2);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            val = BitConverter.ToInt16(tmp);
          } break;
          case "UInt16": { // 2 bytes
            var tmp = reader.ReadBytes(2);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
              val = BitConverter.ToUInt16(tmp);
          } break;
          case "Int32": { // 4 bytes
            var tmp = reader.ReadBytes(4);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            val = BitConverter.ToInt32(tmp);
          } break;
          case "UInt32": { // 4 bytes
            var tmp = reader.ReadBytes(4);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
              val = BitConverter.ToUInt32(tmp);
          } break;
          case "Int64": { // 8 bytes
            var tmp = reader.ReadBytes(8);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            val = BitConverter.ToInt64(tmp);
          } break;
          case "UInt64": {
            var tmp = reader.ReadBytes(8);
              if (Endian == '>') {
               Array.Reverse(tmp);
              }
              val = BitConverter.ToUInt64(tmp);
            } break;
          case "Half": { // 2 bytes
            var tmp = reader.ReadBytes(2);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            val = BitConverter.ToHalf(tmp);
          } break; 
          case "Single": { // 4 bytes
            var tmp = reader.ReadBytes(4);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            val = BitConverter.ToSingle(tmp);
          } break;
          case "Double": { // 8 bytes
            var tmp = reader.ReadBytes(8);
            if (Endian == '>') {
               Array.Reverse(tmp);
            }
            val = BitConverter.ToDouble(tmp);
          } break;
          case "String": { // variable
             byte[] stringBytes = reader.ReadBytes(valueType.size);
             val = Encoding.UTF8.GetString(stringBytes);
          } break;
        }
        returnList.Add(val);
      }


      return returnList;
    }


    public static int calcSize(string format) {
      var types = getFormatTypeList(format);
      int totalSize = 0;
      foreach (var valueType in types) {
        totalSize += valueType.size;
      }
      return totalSize;
    }

    static List<TypeSpecifier> getFormatTypeList (string format) {
      List<TypeSpecifier> FormatTypeList = [];

      if (format.Length == 0) {
        return FormatTypeList;
      }

      int i = 0;
      char endian = format[0];

      if (ValidEndians.Contains(endian)) {
        Debug.WriteLine("Skipping Endian Marker " + endian);
        i++;
      }

      string tempCount = "";


      while (i < format.Length) {
        char specifier = format[i];
        i++;

        int count = 1;
        int size = 1;
        string type = "Byte";

        if (Char.IsDigit(specifier)) {
          tempCount += specifier;
          continue;
        }

        if (tempCount.Length > 0) {
          count = int.Parse(tempCount);
          tempCount = "";
        }

        switch (specifier) {
          case 'x': { // Pad byte
            type = "Pad";
            size = 1;
            } break;
          case 'c': { // char
            type = "Char";
            size = 1;
            } break;
          case 'b': { // signed char ?
            type = "SByte";
            size = 1;
            } break;
          case 'B': { // unsigned char ?
            type = "Byte";
            size = 1;
            } break;
          case '?': { // _Bool
            type = "Boolean";
            size = 1;
            } break;
          case 'h': { // short
            type = "Int16";
            size = 2;
            } break;
          case 'H': { // unsigned short
            type = "UInt16";
            size = 2;
            } break;
          case 'i': { // int
            type = "Int32";
            size = 4;
            } break;
          case 'I': { // unsigned int
            type = "UInt32";
            size = 4;
            } break;
          case 'l': { // long
            type = "Int32";
            size = 4;
            } break;
          case 'L': { // unsigned long
            type = "UInt32";
            size = 4;
            } break;
          case 'q': { // long long
            type = "Int64";
            size = 8;
            } break;
          case 'Q': { // unsigned long long
            type = "UInt64";
            size = 8;
            } break;
          case 'e': {
            type = "Half";
            size = 2;
            } break;
          case 'f': { // float
            type = "Single";
            size = 4;
            } break;
          case 'd': { // double
            type = "Double";
            size = 8;
            } break;
          case 's': { // string
            type = "String";
            size = count;
            count = 1;
            } break;
          default:
            throw new ArgumentException("Invalid Format Specifier: " + specifier);
        }

        for (int k = 0; k < count; k++) {
          var temp = new TypeSpecifier();
          temp.size = size;
          temp.type = type;

          FormatTypeList.Add(temp);
        }
      }
      return FormatTypeList;
    }
  }
}
