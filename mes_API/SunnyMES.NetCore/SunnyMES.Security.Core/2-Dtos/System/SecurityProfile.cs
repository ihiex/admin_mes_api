using AutoMapper;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Options;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityProfile : Profile
    {
        /// <summary>
        /// /
        /// </summary>
        public SecurityProfile()
        {
            CreateMap<APP, AppOutputDto>();
            CreateMap<APPInputDto, APP>();
            CreateMap<APP, AllowCacheApp>();
            CreateMap<Area, AreaOutputDto>();
            CreateMap<Area, AreaPickerOutputDto>()
                .ForMember(s=>s.label,s=>s.MapFrom(o=>o.FullName))
                .ForMember(s => s.value, s => s.MapFrom(o => o.Id));
            CreateMap<Area, AreaSelect2OutDto>()
                .ForMember(e => e.text, s => s.MapFrom(o => o.FullName))
                .ForMember(e => e.id, s => s.MapFrom(o => o.Id));
            CreateMap<AreaInputDto, Area>();
            CreateMap<ItemsDetail, ItemsDetailOutputDto>();
            CreateMap<ItemsDetailInputDto, ItemsDetail>();
            CreateMap<Items, ItemsOutputDto>();
            CreateMap<ItemsInputDto, Items>();
            CreateMap<Menu, MenuOutputDto>();
            CreateMap<Menu, MenuTreeTableOutputDto>();
            CreateMap<Menu, ModuleFunctionOutputDto>()
                .ForMember(s => s.Id, s => s.MapFrom(o => o.Id))
                .ForMember(s=>s.FullName,s=>s.MapFrom(o=>o.FullName));
            CreateMap<MenuInputDto, Menu>();
            CreateMap<Organize, OrganizeOutputDto>();
            CreateMap<OrganizeInputDto, Organize>();
            CreateMap<Role, RoleOutputDto>();
            CreateMap<RoleInputDto, Role>();
            CreateMap<SystemType, SystemTypeOutputDto>();
            CreateMap<SystemTypeInputDto, SystemType>();
            CreateMap<UploadFile, UploadFileOutputDto>();
            CreateMap<UploadFileInputDto, UploadFile>();
            CreateMap<UploadFile, UploadFileResultOuputDto>();
            CreateMap<User, UserOutputDto>().ReverseMap();
            CreateMap<UserInputDto, User>();
            CreateMap<User, UserLoginDto>()
                .ForMember(e => e.UserId, s => s.MapFrom(o => o.Id));
            CreateMap<UserExtend, UserExtendOutputDto>();
            CreateMap<Log, LogOutputDto>();
            CreateMap<LogInputDto, Log>();
            CreateMap<FilterIP, FilterIPOutputDto>();
            CreateMap<FilterIPInputDto, FilterIP>();
            CreateMap<SysSetting, SysSettingOutputDto>();

            CreateMap<Sequence, SequenceOutputDto>();
            CreateMap<SequenceInputDto, Sequence>();
            CreateMap<SequenceRule, SequenceRuleOutputDto>();
            CreateMap<SequenceRuleInputDto, SequenceRule>();


            CreateMap<AZhang, AZhangOutputDto>();
            CreateMap<AZhangInputDto, AZhang>();

            CreateMap<Zhang, ZhangOutputDto>();
            CreateMap<ZhangInputDto, Zhang>();

            CreateMap<BZhang, BZhangOutputDto>();
            CreateMap<BZhangInputDto, BZhang>();

            CreateMap<Maxdata, MaxdataOutputDto>();
            CreateMap<MaxdataInputDto, Maxdata>();

            CreateMap<BomPartInfo, BomLinkedInfo>();
            //CreateMap<SC_luPartFamilyType, SC_luPartFamilyTypeListOutputDto>();
            //CreateMap<SC_luPartFamily, SC_luPartFamilyListOutputDto>();
            //CreateMap<SC_mesPart, SC_mesPartListOutputDto>();
            CreateMap<PMCPOInsertInputDto, SC_mesProductionOrder>();
        }
    }
}
