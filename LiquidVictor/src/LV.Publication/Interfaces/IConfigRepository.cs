using LV.Publication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Interfaces
{
    public interface IConfigRepository
    {
        Configuration GetConfig();
    }
}
