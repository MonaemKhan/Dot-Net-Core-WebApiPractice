using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClassObjectMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping Entity → DTO
            CreateMap<Medicine,MedicineEntity>()
                .ForMember(dest => dest.MedicineId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.BatchNo, opt => opt.Ignore()); // Ignore if not in source
        }
    }
}
