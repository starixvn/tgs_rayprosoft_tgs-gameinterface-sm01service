using System.Security.Cryptography;
using System.Text;

namespace Tgs.GameInterface.SM01Service.Core;

public class SM01Hasher
{
    private static readonly MD5 _Md5;

    static SM01Hasher()
    {
        _Md5 = MD5.Create();
    }

    public static string ComputeHash(string merchantKey, IDictionary<string, string> dict)
    {
        var content = string.Join("&", dict
                    .OrderBy(x => x.Key, StringComparer.Ordinal)
                    .Select(p =>
                    {
                        return p.Value is null ? $"{p.Key}=" : $"{p.Key}={p.Value}";
                    }));

        var plainText = $"{content}&secret_key={merchantKey}";

        var data = Encoding.UTF8.GetBytes(plainText);

        var hash = _Md5.ComputeHash(data);

        return Convert.ToHexString(hash).ToUpper();
    }
}
