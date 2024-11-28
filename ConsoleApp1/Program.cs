using System.Globalization;

Console.WriteLine((DateTime.Now - DateTime.UnixEpoch).TotalSeconds.ToString(CultureInfo.InvariantCulture));