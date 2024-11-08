try
{
    var config = ConfigReader.ReadConfig();
    ConfigValidator.ValidateAndCorrect(config);
    await Task.WhenAll(config.Mods.Select(mod => ModProcessor.Process(config, mod)));
    ConsoleHelper.LogInformation(ConfigParameterName.StalkerModdingHelper, "Done.");
    Launcher.Launch(config);
}
catch (Exception e)
{
    Console.WriteLine(e);
    Console.ReadKey();
}