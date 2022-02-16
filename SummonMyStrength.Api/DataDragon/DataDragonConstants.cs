namespace SummonMyStrength.Api.DataDragon
{
    internal static class DataDragonConstants
    {
        public const string BaseUrl = "http://ddragon.leagueoflegends.com/cdn/12.3.1/";

        public const string ImageUrl = BaseUrl + "img/";
        public const string ChampionUrl = BaseUrl + "data/en_US/champion/";
        public const string ChampionSplashArtUrl = "http://ddragon.leagueoflegends.com/cdn/img/champion/splash/"; // last url part is "{ChampionName}_{SkinIndex}.png"

        public const string SummonerSpellDataUrl = BaseUrl + "data/en_US/summoner.json";
        public const string ChampionsDataUrl = BaseUrl + "data/en_US/champion.json";
    }
}
