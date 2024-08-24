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
            CreateMap<Credential, CredentialDTO>();

            AllowNullCollections = true;
        }
    }
}
