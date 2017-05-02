namespace Cadena.Meteor._Internals
{
    internal interface IKeyCacheTreeDigger
    {
        int ItemValidLength { get; }

        string PointingItem { get; }

        void Initialize();

        bool DigNextChar(char c);

        void Complete();
    }
}