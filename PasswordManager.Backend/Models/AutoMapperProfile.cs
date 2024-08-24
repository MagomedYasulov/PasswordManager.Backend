using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordManager.Backend.Data.Entities;
using PasswordManager.Backend.DTOs;
using System.Data;

namespace PasswordManager.Backend.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
                //.ForMember(userDto => userDto.IsAdmin,
                //    option => option.MapFrom(user => user.Role.IsAdmin));
                //CreateMap<Camera, CameraDTO>();

            //CreateMap<PageCamera, PageCameraDTO>();
            //CreateMap<Page, PageExtentionDTO>();

            //CreateMap<Grid, GridDTO>();
            //CreateMap<Grid, GridExtentionDTO>();

            //CreateMap<Role, RoleDTO>()
            //    .ForMember(roleDTO => roleDTO.Cameras,
            //        option => option.MapFrom(role => role.Cameras.Select(c => c.CameraId)))
            //    .ForMember(roleDTO => roleDTO.Grids,
            //        option => option.MapFrom(role => role.Grids.Select(g => g.GridId)));

            //CreateMap<Role, RoleExtentionDTO>()
            //    .ForMember(roleDTO => roleDTO.Cameras,
            //        option => option.MapFrom(role => role.Cameras.Where(c => c.Camera != null).Select(c => c.Camera)))
            //    .ForMember(roleDTO => roleDTO.Grids,
            //        option => option.MapFrom(role => role.Grids.Where(g => g.Grid != null).Select(g => g.Grid)));


            //CreateMap<Settings, SettingsDTO>();
            //CreateMap<Settings, SettingsAddressesDTO>();
            //CreateMap<Bind, BindDTO>();
            //CreateMap<ICEServer, ICEServerDTO>();

            //CreateMap<Sadp_Device_Info, SadpDeviceInfoDTO>()
            //    .ForMember(sdiDTO => sdiDTO.SzIPv4Address,
            //        option => option.MapFrom(sdi => sdi.SzIPv4Address.ToString()))
            //    .ForMember(sdiDTO => sdiDTO.SzIPv4SubnetMask,
            //            option => option.MapFrom(sdi => sdi.SzIPv4SubnetMask.ToString()))
            //    .ForMember(sdiDTO => sdiDTO.SzIPv4Gateway,
            //            option => option.MapFrom(sdi => sdi.SzIPv4Gateway.ToString()))
            //    .ForMember(sdiDTO => sdiDTO.SzIPv6Address,
            //            option => option.MapFrom(sdi => sdi.SzIPv6Address.ToString()))
            //    .ForMember(sdiDTO => sdiDTO.SzIPv6Gateway,
            //            option => option.MapFrom(sdi => sdi.SzIPv6Gateway.ToString()));

            //CreateMap<MapCamera, MapCameraDTO>();

            AllowNullCollections = true;
        }
    }
}
