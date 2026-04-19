using System;
using System.Collections.Generic;
using System.Linq;

namespace QobuzDownloaderX.Shared
{
    internal class PerformersParser
    {
        private readonly Dictionary<string, List<string>> performers;

        public PerformersParser(string performersFullString)
        {
            performers = new Dictionary<string, List<string>>();
            if (!string.IsNullOrEmpty(performersFullString))
            {
                performers = performersFullString
                    .Split(new string[] { " - " }, StringSplitOptions.None) // Split performers by " - " because some roles include '-'
                    .Select(performer => performer.Split(',')) // Split name & roles in best effort by ',', first part is name, next parts roles
                    .GroupBy(parts => parts[0].Trim()) // Group performers by name since they can occur multiple times
                    .ToDictionary(group => group.Key, group => group.SelectMany(parts =>
                        parts.Skip(1).Select(role => role.Trim())).Distinct().ToList()); // Flatten roles by performer and remove duplicates
            }
        }

        public string[] GetPerformersWithRole(InvolvedPersonRoleType role)
        {
            var roleStrings = InvolvedPersonRoleMapping.GetStringsByRole(role);
            return performers.Keys
                .Where(key => performers[key].Exists(value => roleStrings.Contains(value, StringComparer.OrdinalIgnoreCase)))
                .ToArray();
        }
    }
}