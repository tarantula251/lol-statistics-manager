using Newtonsoft.Json;
using LOLStatisticsManager.Controller;
using System.Collections.Generic;
using System;

namespace LOLStatisticsManager.Model
{
    class StatisticsController
    {
        private RiotAPIController RiotApiController { get; set; }
        private string Region { get; set; }
        private string SummonerName { get; set; }
        private string AccountId { get; set; }
        private string SummonerId { get; set; }
        private string MatchId { get; set; }

        private string QueueTypeSolo = "SOLO";

        private string QueueTypeFlex = "FLEX";

        public StatisticsController(RiotAPIController apiController, string region, string summonerName)
        {
            this.RiotApiController = apiController;
            this.Region = region;
            this.SummonerName = summonerName;
        }

        public Dictionary<string, string> GetSummonerData()
        {
            SummonerDTO summoner = RiotApiController.GetSummonerByName(SummonerName);
            long unixDate = summoner.RevisionDate;
            DateTime startdate = DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime;
            Dictionary<string, string> resultMap = new Dictionary<string, string>()
            {
                { "name", summoner.Name },
                { "level", summoner.SummonerLevel.ToString() },
                { "lastUpdated",  startdate.ToString()},
                { "profileIconId", summoner.ProfileIconId.ToString() }
            };

            AccountId = summoner.AccountId;
            SummonerId = summoner.Id;

            return resultMap;                                   
        }

        public List<Dictionary<string, string>> GetLeagueEntryData()
        {
            List<LeagueEntryDTO> leagueEntryList = RiotApiController.GetEntryBySummoner(SummonerId);
            List<Dictionary<string, string>> resultList = new List<Dictionary<string, string>>();
            if (leagueEntryList != null && leagueEntryList.Count > 0)
            {
                foreach(LeagueEntryDTO entry in leagueEntryList)
                {
                    Dictionary<string, string> leagueEntryDictionary = new Dictionary<string, string>()
                    {
                        { "rankData", entry.Tier + "  " + entry.Rank },
                        { "points", entry.LeaguePoints.ToString() + " LP" },
                        { "wins", entry.Wins.ToString() + " W" },
                        { "losses", entry.Losses.ToString() + " L" },
                        { "efficiency", (100 * entry.Wins / (entry.Losses + entry.Wins)).ToString() + " %" },
                        { "tier", entry.Tier }
                    };

                    if (entry.QueueType.Contains(QueueTypeSolo))
                    {
                        leagueEntryDictionary.Add("rankType", "Ranked Solo");
                    }
                    else if (entry.QueueType.Contains(QueueTypeFlex))
                    {
                        leagueEntryDictionary.Add("rankType", "Ranked Flex");
                    }

                    resultList.Add(leagueEntryDictionary);
                }
            }

            return resultList;
        }

        public Dictionary<string, object> GetMatchReferenceData()
        {
            MatchlistDTO matchlist = RiotApiController.GetMatchlistByAccount(AccountId);
            List<Dictionary<string, object>> matchReferenceList = new List<Dictionary<string, object>>();
            if (matchlist.Matches != null)
            {
                foreach(MatchReferenceDTO matchReference in matchlist.Matches)
                {
                    Dictionary<string, object> reference = new Dictionary<string, object>()
                    {
                        { "lane", matchReference.Lane },
                        { "gameId", matchReference.GameId },
                        { "champion", matchReference.Champion },
                        { "platformId", matchReference.PlatformId },
                        { "season", matchReference.Season },
                        { "queue", matchReference.Queue },
                        { "role", matchReference.Role },
                        { "timestamp", matchReference.Timestamp },
                    };
                    matchReferenceList.Add(reference);
                }

                Dictionary<string, object> test = matchReferenceList[0];
                Console.WriteLine("1 " + test["lane"]);
                Console.WriteLine("1 " + test["gameId"]);
                Console.WriteLine("1 " + test["champion"]);
                Console.WriteLine("1 " + test["platformId"]);
                Console.WriteLine("1 " + test["season"]);
                Console.WriteLine("1 " + test["queue"]);
                Console.WriteLine("1 " + test["role"]);
                Console.WriteLine("1 " + test["timestamp"]);
                Console.WriteLine("----------------");

                //to be fixed
                MatchId = test["gameId"].ToString();
            }

            Dictionary<string, object> resultMap = new Dictionary<string, object>()
            {
                { "totalGames", matchlist.TotalGames.ToString() },
                { "startIndex", matchlist.StartIndex.ToString() },
                { "endIndex",  matchlist.EndIndex.ToString()},
                { "matches", matchReferenceList }
            };

            Console.WriteLine("1 " + resultMap["totalGames"]);
            Console.WriteLine("1 " + resultMap["startIndex"]);
            Console.WriteLine("1 " + resultMap["endIndex"]);

            return resultMap;
        }

        public Dictionary<string, object> GetMatchData()
        {
            MatchDTO match = RiotApiController.GetMatchEntryByMatch(MatchId);
            
            Dictionary<string, object> resultMap = new Dictionary<string, object>()
            {
                { "seasonId", match.SeasonId.ToString() },
                { "queueId", match.QueueId.ToString() },
                { "gameVersion", match.GameVersion },
                { "platformId", match.PlatformId },
                { "platformId", match.PlatformId },
                { "gameMode", match.GameMode },
                { "mapId", match.MapId.ToString() },
                { "gameType", match.GameType },
                { "gameDuration", match.GameDuration.ToString() },
                { "gameCreation", match.GameCreation.ToString() },
                { "participants", GetParticipantIdentities(match) },
                { "teams", GetTeamStats(match) },
                { "participants", GetParticipants(match) },
            };
            
            return resultMap;
        }

        private List<Dictionary<string, object>> GetParticipantIdentities(MatchDTO match)
        {
            List<Dictionary<string, object>> participantsList = new List<Dictionary<string, object>>();
            if (match.ParticipantIdentities != null)
            {
                foreach (ParticipantIdentityDTO participantIdentity in match.ParticipantIdentities)
                {
                    Dictionary<string, object> player = new Dictionary<string, object>()
                    {
                        { "currentPlatformId", participantIdentity.Player.CurrentPlatformId },
                        { "summonerName", participantIdentity.Player.SummonerName },
                        { "matchHistoryUri", participantIdentity.Player.MatchHistoryUri },
                        { "platformId", participantIdentity.Player.PlatformId },
                        { "currentAccountId", participantIdentity.Player.CurrentAccountId },
                        { "profileIcon", participantIdentity.Player.ProfileIcon },
                        { "summonerId", participantIdentity.Player.SummonerId },
                        { "accountId", participantIdentity.Player.AccountId },
                    };

                    Dictionary<string, object> participant = new Dictionary<string, object>()
                    {
                        { "participantId", participantIdentity.ParticipantId },
                        { "player", player }
                    };
                    participantsList.Add(participant);
                }

                Dictionary<string, object> testParticipant = participantsList[0];
                Dictionary<string, object> test = (Dictionary<string, object>)testParticipant["player"];
                Console.WriteLine("1 " + test["currentPlatformId"]);
                Console.WriteLine("1 " + test["summonerName"]);
                Console.WriteLine("1 " + test["matchHistoryUri"]);
                Console.WriteLine("1 " + test["platformId"]);
                Console.WriteLine("1 " + test["currentAccountId"]);
                Console.WriteLine("1 " + test["profileIcon"]);
                Console.WriteLine("1 " + test["summonerId"]);
                Console.WriteLine("1 " + test["accountId"]);
                Console.WriteLine("1 " + testParticipant["participantId"]);
                Console.WriteLine("----------------");
            }

            return participantsList;
        }

        private List<Dictionary<string, object>> GetTeamStats(MatchDTO match)
        {
            List<Dictionary<string, object>> teamStatsList = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> teamBans = new List<Dictionary<string, object>>();
            if (match.Teams != null)
            {
                foreach (TeamStatsDTO teamStat in match.Teams)
                {
                    foreach (TeamBansDTO teamBan in teamStat.Bans)
                    {
                        Dictionary<string, object> ban = new Dictionary<string, object>()
                        {
                            { "pickTurn", teamBan.PickTurn },
                            { "championId", teamBan.ChampionId }
                        };
                        teamBans.Add(ban);
                    }

                    Dictionary<string, object> stat = new Dictionary<string, object>()
                    {
                        { "firstDragon", teamStat.FirstDragon },
                        { "firstInhibitor", teamStat.FirstInhibitor },
                        { "bans", teamBans },
                        { "baronKills", teamStat.BaronKills },
                        { "firstRiftHerald", teamStat.FirstRiftHerald },
                        { "firstBaron", teamStat.FirstBaron },
                        { "riftHeraldKills", teamStat.RiftHeraldKills },
                        { "firstBlood", teamStat.FirstBlood },
                        { "teamId", teamStat.TeamId },
                        { "firstTower", teamStat.FirstTower },
                        { "vilemawKills", teamStat.VilemawKills },
                        { "inhibitorKills", teamStat.InhibitorKills },
                        { "towerKills", teamStat.TowerKills },
                        { "dominionVictoryScore", teamStat.DominionVictoryScore },
                        { "win", teamStat.Win },
                        { "dragonKills", teamStat.DragonKills },
                    };
                    teamStatsList.Add(stat);
                }

                Dictionary<string, object> testTeamstats = teamStatsList[0];
                Dictionary<string, object> test = (Dictionary<string, object>)testTeamstats["bans"];
                Console.WriteLine("1 " + test["pickTurn"]);
                Console.WriteLine("1 " + test["championId"]);
            }

            return teamStatsList;
        }

        private List<Dictionary<string, object>> GetParticipants(MatchDTO match)
        {
            List<Dictionary<string, object>> participantsList = new List<Dictionary<string, object>>();
            if (match.Participants != null)
            {
                foreach (ParticipantDTO participantDTO in match.Participants)
                {
                    Dictionary<string, object> stats = GetStatsForParticipantDTO(participantDTO.Stats);
                    List<Dictionary<string, object>> runes = GetRunesForParticipantDTO(participantDTO);
                    Dictionary<string, object> timeline = GetTimelineForParticipantDTO(participantDTO);
                    List<Dictionary<string, object>> masteries = GetMasteriesForParticipantDTO(participantDTO);

                    Dictionary<string, object> participant = new Dictionary<string, object>()
                    {
                        { "participantId", participantDTO.ParticipantId },
                        { "teamId", participantDTO.TeamId },
                        { "spell2Id", participantDTO.Spell2Id },
                        { "highestAchievedSeasonTier", participantDTO.HighestAchievedSeasonTier },
                        { "spell1Id", participantDTO.Spell1Id },
                        { "championId", participantDTO.ChampionId },
                        { "stats", stats },
                        { "runes", runes },
                        { "timeline", timeline },
                        { "masteries", masteries }

                    };
                    participantsList.Add(participant);
                }

                Dictionary<string, object> testparticipant = participantsList[0];

            }
            return participantsList;
        }

        private Dictionary<string, object> GetStatsForParticipantDTO(ParticipantStatsDTO participantStats)
        {
            Dictionary<string, object> resultMap = new Dictionary<string, object>()
            {
                { "firstBloodAssist", participantStats.FirstBloodAssist },
                { "visionScore", participantStats.VisionScore }
                // ... to be continued

            };
            return resultMap;
        }

        private List<Dictionary<string, object>> GetRunesForParticipantDTO(ParticipantDTO participantDTO)
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            if (participantDTO.Runes != null && participantDTO.Runes.Count > 0)
            {
                foreach(RuneDTO rune in participantDTO.Runes)
                {
                    Dictionary<string, object> runeMap = new Dictionary<string, object>()
                    {
                        { "runeId", rune.RuneId },
                        { "rank", rune.Rank }
                    };
                    resultList.Add(runeMap);
                }
            }
            return resultList;
        }

        private Dictionary<string, object> GetTimelineForParticipantDTO(ParticipantDTO participantDTO)
        {              
            Dictionary<string, object> resultMap = new Dictionary<string, object>()
            {
                { "lane", participantDTO.Timeline.Lane },
                { "participantId", participantDTO.Timeline.ParticipantId },
                { "csDiffPerMinDeltas", participantDTO.Timeline.CsDiffPerMinDeltas },
                { "goldPerMinDeltas", participantDTO.Timeline.GoldPerMinDeltas },
                { "xpDiffPerMinDeltas", participantDTO.Timeline.XpDiffPerMinDeltas },
                { "creepsPerMinDeltas", participantDTO.Timeline.CreepsPerMinDeltas },
                { "xpPerMinDeltas", participantDTO.Timeline.XpPerMinDeltas },
                { "role", participantDTO.Timeline.Role },
                { "damageTakenDiffPerMinDeltas", participantDTO.Timeline.DamageTakenDiffPerMinDeltas },
                { "damageTakenPerMinDeltas", participantDTO.Timeline.DamageTakenPerMinDeltas },
            };                            
            return resultMap;
        }

        private List<Dictionary<string, object>> GetMasteriesForParticipantDTO(ParticipantDTO participantDTO)
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            if (participantDTO.Masteries != null && participantDTO.Masteries.Count > 0)
            {
                foreach (MasteryDTO mastery in participantDTO.Masteries)
                {
                    Dictionary<string, object> masteryMap = new Dictionary<string, object>()
                    {
                        { "masteryId", mastery.MasteryId },
                        { "rank", mastery.Rank }
                    };
                    resultList.Add(masteryMap);
                }
            }
            return resultList;
        }
        public List<Dictionary<string, object>> GetChampionMasteryData()
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            List<ChampionMasteryDTO> championMasteryList = RiotApiController.GetChampionEntryBySummoner(SummonerId);
            if (championMasteryList != null && championMasteryList.Count > 0)
            {
                foreach(ChampionMasteryDTO championMastery in championMasteryList)
                {
                    Dictionary<string, object> championMasteryMap = new Dictionary<string, object>()
                    {
                        { "chestGranted", championMastery.ChestGranted },
                        { "championLevel", championMastery.ChampionLevel },
                        { "championPoints", championMastery.ChampionPoints },
                        { "championId", championMastery.ChampionId},
                        { "championPointsUntilNextLevel", championMastery.ChampionPointsUntilNextLevel },
                        { "lastPlayTime", championMastery.LastPlayTime},
                        { "tokensEarned", championMastery.TokensEarned },
                        { "championPointsSinceLastLevel", championMastery.ChampionPointsSinceLastLevel },
                        { "summonerId", championMastery.SummonerId }
                    };
                    resultList.Add(championMasteryMap);
                }
            }
            return resultList;
        }
    }
}
