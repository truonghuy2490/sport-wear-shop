using SportWearShop.BusinessLogics.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Helpers;

public static class EnumHelper
{
    public static T ParseEnum<T>(string input) where T : struct, Enum
    {
        if (!Enum.TryParse<T>(input, true, out var value))
            throw new BadRequestException($"Invalid {typeof(T).Name}");

        return value;
    }
}