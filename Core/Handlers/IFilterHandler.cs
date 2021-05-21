using Core.Entities;
using Core.Filters;

namespace Core.Handlers
{
    public interface IFilterHandler
    {
        public BiQuadFilter[,] Filters { get; set; }
        public float[] Gains { get; set; }
        public void CreateFilters();
    }
}
