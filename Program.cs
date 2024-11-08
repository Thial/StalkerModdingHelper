try
{
    var config = ConfigReader.ReadConfig();
    ConfigValidator.ValidateAndCorrect(config);
    foreach (var mod in config.Mods)
        await ModProcessor.Process(config, mod);
    ConsoleHelper.LogInformation(ConfigParameterName.StalkerModdingHelper, "Done.");
    Launcher.Launch(config);
}
catch (Exception e)
{
    Console.WriteLine(e);
    Console.ReadKey();
}