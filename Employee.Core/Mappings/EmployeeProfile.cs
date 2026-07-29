using Employee.Core.DTO;
using Employee.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Mappings
{
    public class EmployeeProfile : AutoMapper.Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Domain.Employee, DTO.EmployeeResponseDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name.Value))

                .ForMember(
                    dest => dest.Surname,
                    opt => opt.MapFrom(src => src.Surname.Value))

                .ForMember(
                    dest => dest.Salary,
                    opt => opt.MapFrom(src => src.Salary.Amount));

            CreateMap<User, UserResponseDto>()
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src => src.Username.Value))

                .ForMember(
                    dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
