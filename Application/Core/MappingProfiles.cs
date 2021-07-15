﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(x => x.HostUsername,
                    o => o.MapFrom
                    (s => s.Attendees
                        .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(d => d.DisplayName,
                    o => o.MapFrom(s => s.AppUser.DisplayName)
                )
                .ForMember(d => d.Bio,
                    o => o.MapFrom(s => s.AppUser.Bio)
                )
                .ForMember(d => d.Username,
                    o => o.MapFrom(s => s.AppUser.UserName)
                ).ForMember(i => i.Image,
                    x => x
                        .MapFrom(p => p.AppUser.Photos
                            .FirstOrDefault(m => m.IsMain).Url));


            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(i => i.Image,
                    x => x
                        .MapFrom(p => p.Photos
                            .FirstOrDefault(m => m.IsMain).Url));

            CreateMap<Comment, CommentDto>()
                .ForMember(u => u.Username,
                    x => x.MapFrom(u => u.Author.UserName))
                .ForMember(u => u.DisplayName,
                    x => x.MapFrom(u => u.Author.DisplayName))
                .ForMember(u => u.Image,
                    x => x.MapFrom(u => u.Author.Photos.FirstOrDefault(p => p.IsMain).Url));

        }
    }
}
