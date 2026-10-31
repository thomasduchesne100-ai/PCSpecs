using System.Management;

namespace PCSpecs;

public static class SystemInfoService
{
    // Icônes (tracés vectoriels, viewbox 24x24)
    private const string IconCpu = "M8,6 H16 A2,2 0 0 1 18,8 V16 A2,2 0 0 1 16,18 H8 A2,2 0 0 1 6,16 V8 A2,2 0 0 1 8,6 Z M9.5,9.5 H14.5 V14.5 H9.5 Z M9,2 V5 M15,2 V5 M9,19 V22 M15,19 V22 M2,9 H5 M2,15 H5 M19,9 H22 M19,15 H22";
    private const string IconGpu = "M4,7 H20 A2,2 0 0 1 22,9 V15 A2,2 0 0 1 20,17 H4 A2,2 0 0 1 2,15 V9 A2,2 0 0 1 4,7 Z M9,9.5 A2.5,2.5 0 1 1 9,14.5 A2.5,2.5 0 1 1 9,9.5 Z M16,9.5 A2.5,2.5 0 1 1 16,14.5 A2.5,2.5 0 1 1 16,9.5 Z M5,17 V19";
    private const string IconRam = "M3.5,8 H20.5 A1.5,1.5 0 0 1 22,9.5 V14.5 A1.5,1.5 0 0 1 20.5,16 H3.5 A1.5,1.5 0 0 1 2,14.5 V9.5 A1.5,1.5 0 0 1 3.5,8 Z M6,11.5 V12.5 M10,11.5 V12.5 M14,11.5 V12.5 M18,11.5 V12.5 M5,16 V18 M9,16 V18 M13,16 V18 M17,16 V18";
    private const string IconBoard = "M5,3 H19 A2,2 0 0 1 21,5 V19 A2,2 0 0 1 19,21 H5 A2,2 0 0 1 3,19 V5 A2,2 0 0 1 5,3 Z M8,7 H12 A1,1 0 0 1 13,8 V12 A1,1 0 0 1 12,13 H8 A1,1 0 0 1 7,12 V8 A1,1 0 0 1 8,7 Z M16,7 H17 M16,11 H17 M7,17 H13 M16,17 H17";
    private const string IconStorage = "M4,6 A8,3 0 1 1 20,6 A8,3 0 1 1 4,6 Z M4,6 V18 A8,3 0 0 0 20,18 V6 M4,12 A8,3 0 0 0 20,12";
    private const string IconWindows = "M3,5.5 L10.5,4.5 V11.5 H3 Z M12,4.3 L21,3 V11.5 H12 Z M3,12.5 H10.5 V19.5 L3,18.5 Z M12,12.5 H21 V21 L12,19.7 Z";

    private static IEnumerable<ManagementObject> Query(string wql)
    {
        using var searcher = new ManagementObjectSearcher(wql);
        foreach (ManagementObject obj in searcher.Get())
            yield return obj;
    }

    private static string Gb(double bytes) => $"{bytes / 1024d / 1024d / 1024d:0.#} GB";

    public static List<SpecItem> GetAllSpecs() => new()
    {
        GetCpu(),
        GetGpu(),
        GetRam(),
        GetMotherboard(),
        GetStorage(),
        GetWindows()
    };

    private static SpecItem GetCpu()
    {
        try
        {
            foreach (var m in Query("SELECT * FROM Win32_Processor"))
            {
                var name = m["Name"]?.ToString()?.Trim() ?? "Inconnu";
                var cores = m["NumberOfCores"];
                var threads = m["NumberOfLogicalProcessors"];
                var ghz = Convert.ToDouble(m["MaxClockSpeed"] ?? 0) / 1000d;
                return new SpecItem
                {
                    IconData = IconCpu,
                    Title = "CPU",
                    Main = name,
                    Details = $"{cores} cœurs • {threads} threads • {ghz:0.00} GHz"
                };
            }
        }
        catch { }
        return new SpecItem { IconData = IconCpu, Title = "CPU", Main = "Non disponible" };
    }

    private static SpecItem GetGpu()
    {
        try
        {
            var names = new List<string>();
            var drivers = new List<string>();
            foreach (var m in Query("SELECT * FROM Win32_VideoController"))
            {
                names.Add(m["Name"]?.ToString()?.Trim() ?? "Inconnu");
                drivers.Add($"Pilote {m["DriverVersion"]}");
            }
            if (names.Count > 0)
                return new SpecItem
                {
                    IconData = IconGpu,
                    Title = "GPU",
                    Main = string.Join("\n", names),
                    Details = string.Join(" • ", drivers)
                };
        }
        catch { }
        return new SpecItem { IconData = IconGpu, Title = "GPU", Main = "Non disponible" };
    }

    private static SpecItem GetRam()
    {
        try
        {
            double total = 0;
            int sticks = 0;
            string speed = "", maker = "";
            foreach (var m in Query("SELECT * FROM Win32_PhysicalMemory"))
            {
                total += Convert.ToDouble(m["Capacity"] ?? 0);
                sticks++;
                speed = m["Speed"]?.ToString() ?? speed;
                maker = m["Manufacturer"]?.ToString()?.Trim() ?? maker;
            }
            if (sticks > 0)
                return new SpecItem
                {
                    IconData = IconRam,
                    Title = "RAM",
                    Main = Gb(total),
                    Details = $"{sticks} barrette(s) • {speed} MHz • {maker}"
                };
        }
        catch { }
        return new SpecItem { IconData = IconRam, Title = "RAM", Main = "Non disponible" };
    }

    private static SpecItem GetMotherboard()
    {
        try
        {
            string bios = "";
            foreach (var b in Query("SELECT * FROM Win32_BIOS"))
            {
                bios = b["SMBIOSBIOSVersion"]?.ToString()?.Trim() ?? "";
                break;
            }

            foreach (var m in Query("SELECT * FROM Win32_BaseBoard"))
            {
                var maker = m["Manufacturer"]?.ToString()?.Trim() ?? "";
                var product = m["Product"]?.ToString()?.Trim() ?? "";
                return new SpecItem
                {
                    IconData = IconBoard,
                    Title = "Motherboard",
                    Main = $"{maker} {product}".Trim(),
                    Details = string.IsNullOrEmpty(bios) ? "" : $"BIOS {bios}"
                };
            }
        }
        catch { }
        return new SpecItem { IconData = IconBoard, Title = "Motherboard", Main = "Non disponible" };
    }

    private static SpecItem GetStorage()
    {
        try
        {
            var lines = new List<string>();
            foreach (var m in Query("SELECT * FROM Win32_DiskDrive"))
            {
                var model = m["Model"]?.ToString()?.Trim() ?? "Disque";
                var size = Convert.ToDouble(m["Size"] ?? 0);
                lines.Add($"{model} — {Gb(size)}");
            }
            if (lines.Count > 0)
                return new SpecItem
                {
                    IconData = IconStorage,
                    Title = "Storage",
                    Main = string.Join("\n", lines),
                    Details = $"{lines.Count} disque(s) détecté(s)"
                };
        }
        catch { }
        return new SpecItem { IconData = IconStorage, Title = "Storage", Main = "Non disponible" };
    }

    private static SpecItem GetWindows()
    {
        try
        {
            foreach (var m in Query("SELECT * FROM Win32_OperatingSystem"))
            {
                var caption = m["Caption"]?.ToString()?.Replace("Microsoft ", "").Trim() ?? "Windows";
                return new SpecItem
                {
                    IconData = IconWindows,
                    Title = "Windows",
                    Main = caption,
                    Details = $"Version {m["Version"]} • Build {m["BuildNumber"]} • {m["OSArchitecture"]}"
                };
            }
        }
        catch { }
        return new SpecItem { IconData = IconWindows, Title = "Windows", Main = "Non disponible" };
    }

    // ----- Pour la page "Performance in use" -----

    public static double GetCpuUsage()
    {
        try
        {
            foreach (var m in Query("SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'"))
                return Convert.ToDouble(m["PercentProcessorTime"]);
        }
        catch { }
        return 0;
    }

    public static double GetGpuUsage()
    {
        try
        {
            var perEngine = new Dictionary<string, double>();
            foreach (var m in Query("SELECT Name, UtilizationPercentage FROM Win32_PerfFormattedData_GPUPerformanceCounters_GPUEngine"))
            {
                var name = m["Name"]?.ToString() ?? "";
                var idx = name.IndexOf("engtype_", StringComparison.OrdinalIgnoreCase);
                if (idx < 0) continue;

                var type = name.Substring(idx + 8);
                var val = Convert.ToDouble(m["UtilizationPercentage"] ?? 0);
                perEngine[type] = perEngine.GetValueOrDefault(type) + val;
            }
            if (perEngine.Count > 0)
                return Math.Min(100, perEngine.Values.Max());
        }
        catch { }
        return 0;
    }

    public static string GetTemperatures()
    {
        try
        {
            var lines = new List<string>();
            using var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            int i = 1;
            foreach (ManagementObject m in searcher.Get())
            {
                double c = Convert.ToDouble(m["CurrentTemperature"]) / 10d - 273.15;
                if (c > 0 && c < 150)
                    lines.Add($"Zone thermique {i} : {c:0} °C");
                i++;
            }
            if (lines.Count > 0)
                return string.Join("\n", lines);
        }
        catch { }
        return "Non disponible";
    }

    public static (double usedGb, double totalGb, double percent) GetRamUsage()
    {
        try
        {
            foreach (var m in Query("SELECT * FROM Win32_OperatingSystem"))
            {
                double totalKb = Convert.ToDouble(m["TotalVisibleMemorySize"]);
                double freeKb = Convert.ToDouble(m["FreePhysicalMemory"]);
                double usedKb = totalKb - freeKb;
                return (usedKb / 1024d / 1024d, totalKb / 1024d / 1024d, usedKb / totalKb * 100d);
            }
        }
        catch { }
        return (0, 0, 0);
    }
}
