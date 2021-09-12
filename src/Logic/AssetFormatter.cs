using System;
using System.Text;

namespace Diversifolio
{
    public sealed class AssetFormatter
    {
        private readonly StringBuilder _stringBuilder;

        public AssetFormatter(StringBuilder stringBuilder) =>
            _stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));

        public static AssetFormatter Shared { get; } = new(new());

        public static void Format(Asset asset, StringBuilder stringBuilder)
        {
            if (asset is null)
                throw new ArgumentNullException(nameof(asset));
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));

            UncheckedFormat(asset, stringBuilder);
        }

        public string Format(Asset asset)
        {
            if (asset is null)
                throw new ArgumentNullException(nameof(asset));

            _stringBuilder.Clear();
            UncheckedFormat(asset, _stringBuilder);
            string result = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return result;
        }

        private static void UncheckedFormat(Asset asset, StringBuilder stringBuilder) =>
            stringBuilder.Append(asset);
    }
}
