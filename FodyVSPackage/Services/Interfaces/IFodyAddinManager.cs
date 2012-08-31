using System.Collections.Generic;
using FodyVSPackage.Models;

namespace FodyVSPackage.Services
{
    public interface IFodyAddinManager
    {
        IEnumerable<IFodyAddin> GetAddins();
        void ReloadAddins();
        IFodyAddin GetAddin(string name);
    }
}