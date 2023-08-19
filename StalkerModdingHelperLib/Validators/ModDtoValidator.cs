using System;
using System.IO;
using FluentValidation;
using StalkerModdingHelperLib.Enums;
using StalkerModdingHelperLib.Model;
using StalkerModdingHelperLib.Static;

namespace StalkerModdingHelperLib.Validators
{
    public class ModDtoValidator : AbstractValidator<ModDto>
    {
        public ModDtoValidator(ConfigDto config)
        {
            RuleFor(m => m.CopyMethod)
                .Must(c => c is CopyMethod.Folder or CopyMethod.FolderContents)
                .When(m => (m.Copy is true || config?.Instance?.Copy is true) && config?.Instance?.CopyMethod == null)
                .WithMessage("The mod's copy method value is out of range");
            
            RuleFor(m => m.ModPath)
                .NotNull()
                .WithMessage("The mod's path can't be null")
                .NotEmpty()
                .WithMessage("The mod's path can't be empty")
                .Must(IO.IsPathValid)
                .When(m => string.IsNullOrEmpty(m.ModPath) == false)
                .WithMessage("The mod's path has an invalid format. It must be an absolute path.");

            RuleFor(m => m.DestinationPath)
                .NotNull()
                .When(m => (m.Copy is true || config?.Instance?.Copy is true) && string.IsNullOrEmpty(config?.Instance?.InstancePath))
                .WithMessage("The mod's destination path can't be null if copying is enabled and instance's destination path is null or empty")
                .NotEmpty()
                .When(m => (m.Copy is true || config?.Instance?.Copy is true) && string.IsNullOrEmpty(config?.Instance?.InstancePath))
                .WithMessage("The mod's destination path can't be empty if copying is enabled and instance's destination path is null or empty")
                .Must(IO.IsPathValid)
                .When(m => string.IsNullOrEmpty(config?.Instance?.InstancePath))
                .WithMessage("The mod's destination path has an invalid format. It must be an absolute path.");

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
        }
    }
}