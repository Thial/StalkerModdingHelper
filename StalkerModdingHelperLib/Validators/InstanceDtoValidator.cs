using System;
using FluentValidation;
using StalkerModdingHelperLib.Enums;
using StalkerModdingHelperLib.Model;
using StalkerModdingHelperLib.Static;

namespace StalkerModdingHelperLib.Validators
{
    public class InstanceDtoValidator : AbstractValidator<InstanceDto>
    {
        public InstanceDtoValidator()
        {
            RuleFor(i => i.Type)
                .NotNull()
                .WithMessage("The instance type can't be null")
                .Must(i => i is InstanceType.Stalker or InstanceType.ModOrganizer)
                .WithMessage("The instance type is out of range");
            
            RuleFor(i => i.ExecutablePath)
                .NotNull()
                .WithMessage("The executable path can't be null")
                .NotEmpty()
                .WithMessage("The executable path can't be empty")
                .Must(IO.IsPathValid)
                .When(i => string.IsNullOrEmpty(i.ExecutablePath) == false)
                .WithMessage("The executable path has an invalid format. It must be an absolute path.");
            
            RuleFor(i => i.InstancePath)
                .NotNull()
                .WithMessage("The destination path can't be null")
                .NotEmpty()
                .WithMessage("The destination path can't be empty")
                .Must(IO.IsPathValid)
                .When(i => string.IsNullOrEmpty(i.InstancePath) == false)
                .WithMessage("The destination path has an invalid format. It must be an absolute path.");

            RuleFor(m => m.Copy)
                .NotNull()
                .WithMessage("The instance's copy value can't be null");
            
            RuleFor(m => m.CopyMethod)
                .NotNull()
                .WithMessage("The instance's copy method can't be null")
                .Must(c => c is CopyMethod.Folder or CopyMethod.FolderContents)
                .WithMessage("The instance's copy method value is out of range");
            
            RuleForEach(m => m.BlacklistedExtensions)
                .NotNull()
                .WithMessage("The extension value can't be null")
                .NotEmpty()
                .WithMessage("The extension value can't be empty");
            
            RuleForEach(m => m.WhitelistedFolders)
                .NotNull()
                .WithMessage("The extension value can't be null")
                .NotEmpty()
                .WithMessage("The extension value can't be empty");
            
            RuleFor(m => m.AutoLoad)
                .NotNull()
                .WithMessage("The instance's auto load value can't be null");
            
            RuleFor(m => m.SaveName)
                .NotNull()
                .WithMessage("The instance's save name can't be null")
                .NotEmpty()
                .WithMessage("The instance's save name can't be empty");
        }
    }
}