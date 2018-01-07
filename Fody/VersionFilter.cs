using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fody
{
    public class VersionFilter
    {
        private readonly Version _minimum;
        private readonly Version _maximum;
        private readonly bool _minimumInclusive;
        private readonly bool _maximumInclusive;

        public static VersionFilter Parse(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString)) return null;

            string[] parts = filterString.Split(',');
            string minString, maxString;
            bool minInclusive = true, maxInclusive = true;
            switch (parts.Length)
            {
                case 1:
                    minString = maxString = parts[0];
                    break;
                case 2:
                    minString = parts[0];
                    maxString = parts[1];

                    break;
                default:
                    throw new Exception($"'{filterString}' is not a valid version filter string");
            }

            if (minString.StartsWith("(", StringComparison.Ordinal))
            {
                minInclusive = false;
            }

            if (maxString.EndsWith(")", StringComparison.Ordinal))
            {
                maxInclusive = false;
            }

            minString = minString.TrimStart('(', '[').Trim();
            maxString = maxString.TrimEnd(')', ']').Trim();

            try
            {
                return new VersionFilter(new Version(minString), new Version(maxString), minInclusive, maxInclusive);
            }
            catch (Exception e)
            {
                throw new Exception($"'{filterString}' is not a valid version filter string", e);
            }
        }

        private VersionFilter(Version minimum, Version maximum, bool minimumInclusive, bool maximumInclusive)
        {
            _minimum = minimum;
            _maximum = maximum;
            _minimumInclusive = minimumInclusive;
            _maximumInclusive = maximumInclusive;
        }

        public bool IsMatch(System.Version version)
        {
            if (_minimumInclusive ? version < _minimum : version <= _minimum)
            {
                return false;
            }

            if (_maximumInclusive ? version > _maximum : version >= _maximum)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"{(_minimumInclusive ? "[" : "(")}{_minimum},{_maximum}{(_maximumInclusive ? "]" : ")")}";
        }

        private class Version
        {
            private readonly int? _major;
            private readonly int? _minor;
            private readonly int? _build;
            private readonly int? _revision;

            public Version(string version)
            {
                if (string.IsNullOrWhiteSpace(version))
                    return;

                string[] parts = version.Split('.');
                switch (parts.Length)
                {
                    case 4:
                        _revision = ParseValue(parts[3]);
                        goto case 3;
                    case 3:
                        _build = ParseValue(parts[2]);
                        goto case 2;
                    case 2:
                        _minor = ParseValue(parts[1]);
                        goto case 1;
                    case 1:
                        _major = ParseValue(parts[0]);
                        break;
                    default: throw new Exception();
                }

                int? ParseValue(string value)
                {
                    if (string.Equals("*", value, StringComparison.Ordinal))
                        return null;
                    return int.Parse(value);
                }
            }

            public override string ToString()
            {
                return string.Join(".", GetParts());
                IEnumerable<string> GetParts()
                {
                    if (_major != null)
                    {
                        yield return _major.Value.ToString();
                        if (_minor != null)
                        {
                            yield return _minor.Value.ToString();
                            if (_build != null)
                            {
                                yield return _build.Value.ToString();
                                if (_revision != null)
                                {
                                    yield return _revision.Value.ToString();
                                }
                            }
                        }
                    }
                }
            }

            public static bool operator >(System.Version a, Version b)
            {
                if (a.Major > b._major)
                    return true;
                if (a.Major != b._major) return false;

                if (a.Minor > b._minor)
                    return true;
                if (a.Minor != b._minor) return false;

                if (a.Build > b._build)
                    return true;
                if (a.Build != b._build) return false;

                if (a.Revision > b._revision)
                    return true;

                return false;
            }

            public static bool operator <(System.Version a, Version b)
            {
                if (a.Major < b._major)
                    return true;
                if (a.Major != b._major) return false;

                if (a.Minor < b._minor)
                    return true;
                if (a.Minor != b._minor) return false;

                if (a.Build < b._build)
                    return true;
                if (a.Build != b._build) return false;

                if (a.Revision < b._revision)
                    return true;
                return false;
            }

            public static bool operator >=(System.Version a, Version b)
            {
                if (a.Major >= b._major)
                    return true;
                if (a.Minor >= b._minor)
                    return true;
                if (a.Build >= b._build)
                    return true;
                if (a.Revision >= b._revision)
                    return true;
                return false;
            }

            public static bool operator <=(System.Version a, Version b)
            {
                if (a.Major <= b._major)
                    return true;
                if (a.Minor <= b._minor)
                    return true;
                if (a.Build <= b._build)
                    return true;
                if (a.Revision <= b._revision)
                    return true;
                return false;
            }
        }
    }
}