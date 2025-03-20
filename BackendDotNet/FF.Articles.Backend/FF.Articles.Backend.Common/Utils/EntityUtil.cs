using System.Security.Principal;
using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Common.Utils;
public static class EntityUtil
{
    public static int GenerateId()
    {
        return Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
    }
}
