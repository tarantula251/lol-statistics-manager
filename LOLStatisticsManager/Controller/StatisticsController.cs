using Newtonsoft.Json;
using LOLStatisticsManager.Controller;
using System.Collections.Generic;
using System;
using LOLStatisticsManager.Model.Stats;

namespace LOLStatisticsManager.Model
{
    class StatisticsController
    {
        private Cache cache = new Cache();
        private RiotAPIController RiotApiController { get; set; }
        private string SummonerName { get; set; }
        private string AccountId { get; set; }
        private string SummonerId { get; set; }
        private string SummonerUniqueName { get; set; }
        private const string QueueTypeSolo = "SOLO";
        private const string QueueTypeFlex = "FLEX";
        private const string ChampionOnLateStatCacheSubdirectoryName = "champions_statistics";
        private const string SummonerDataCacheSubdirectoryName = "summoner_data";
        private const string LeagueEntriesCacheSubdirectoryName = "legaue_entries";
        public bool CachingEnabled { get; set; }

        public StatisticsController(RiotAPIController apiController, string summonerName)
        {
            this.RiotApiController = apiController;
            this.SummonerName = summonerName;
            this.SummonerUniqueName = SummonerName + "_" + apiController.Region; 
            CachingEnabled = true;
        }

        public List<ChampionOnLaneStat> GetChampionOnLaneStats()
        {
            List<ChampionOnLaneStat> result;

            if (CachingEnabled)
            {
                result = cache.Load<List<ChampionOnLaneStat>>(SummonerUniqueName, ChampionOnLateStatCacheSubdirectoryName);
                if (result != null) return result;
            }

            result = new List<ChampionOnLaneStat>();

            Dictionary<string, Tuple<int, ChampionOnLaneStat>> championOnLaneMap = new Dictionary<string, Tuple<int, ChampionOnLaneStat>>();
            MatchlistDTO matchList = RiotApiController.GetMatchlistByAccount(AccountId);
            int matchesInCurrentSeason = 0;
            foreach(var matchReference in matchList.Matches)
            {
                if (matchReference.Season != 13) continue;
                ++matchesInCurrentSeason;
                ChampionOnLaneStat championOnLaneStat;
                string key = matchReference.Lane + matchReference.Champion;
                if (championOnLaneMap.ContainsKey(key))
                {
                    var championOnLaneTuple = championOnLaneMap[key];
                    championOnLaneStat = championOnLaneTuple.Item2;
                    int championOnLaneCount = championOnLaneTuple.Item1 + 1;
                    championOnLaneMap[key] = new Tuple<int, ChampionOnLaneStat>(championOnLaneCount, championOnLaneStat);
                }
                else
                {
                    championOnLaneStat = new ChampionOnLaneStat();
                    championOnLaneStat.Champion = matchReference.Champion;
                    championOnLaneStat.Lane = matchReference.Lane;
                    championOnLaneMap.Add(key, new Tuple<int, ChampionOnLaneStat>(1, championOnLaneStat));
                }
                MatchDTO match = RiotApiController.GetMatchEntryByMatch(matchReference.GameId);
                if (match == null) continue;
                var participantIdentity = match.ParticipantIdentities.Find((ParticipantIdentityDTO x) =>
                {
                    return x.Player.SummonerId == SummonerId;
                });
                if (participantIdentity == null) continue;

                var participant = match.Participants.Find((ParticipantDTO x) =>
                {
                    return x.ParticipantId == participantIdentity.ParticipantId;
                });
                if (participant == null) continue;

                var participantStats = participant.Stats;

                int gameDurationInMinutes = (int)(match.GameDuration / 60);

                championOnLaneStat.KillsAvg += participantStats.Kills;
                championOnLaneStat.DeathsAvg += participantStats.Deaths;
                championOnLaneStat.AssistsAvg += participantStats.Assists;
                if(participantStats.Win) championOnLaneStat.WinPercent += 1.0f;
                championOnLaneStat.TotalDamageDealtAvgPerMin += participantStats.TotalDamageDealtToChampions / gameDurationInMinutes;
                championOnLaneStat.GoldEarnedAvgPerMin += participantStats.GoldEarned / gameDurationInMinutes;
                championOnLaneStat.MinionsKilledAvgPerMin += participantStats.TotalMinionsKilled / gameDurationInMinutes;
                if (participantStats.FirstBloodAssist || participantStats.FirstBloodKill) championOnLaneStat.FirstBloodParticipationPercent += 1.0f;
                championOnLaneStat.TopItem0 = participantStats.Item0;
                championOnLaneStat.TopItem1 = participantStats.Item1;
                championOnLaneStat.TopItem2 = participantStats.Item2;
                championOnLaneStat.TopItem3 = participantStats.Item3;
                championOnLaneStat.TopItem4 = participantStats.Item4;
                championOnLaneStat.TopItem5 = participantStats.Item5;
                championOnLaneStat.TopItem6 = participantStats.Item6;
                championOnLaneStat.TopSpell1 = participant.Spell1Id;
                championOnLaneStat.TopSpell2 = participant.Spell2Id;
                championOnLaneStat.TopRune0 = participantStats.Perk0;
                championOnLaneStat.TopRune1 = participantStats.Perk1;
                championOnLaneStat.TopRune2 = participantStats.Perk2;
                championOnLaneStat.TopRune3 = participantStats.Perk3;
                championOnLaneStat.TopRune4 = participantStats.Perk4;
                championOnLaneStat.TopRune5 = participantStats.Perk5;
            }

            foreach(var statPair in championOnLaneMap)
            {
                var stat = statPair.Value.Item2;
                int count = statPair.Value.Item1;
                stat.PickPercent = ((float)count / matchesInCurrentSeason) * 100.0f;
                stat.KillsAvg /= count;
                stat.DeathsAvg /= count;
                stat.AssistsAvg /= count;
                stat.WinPercent = (stat.WinPercent / count) * 100.0f;
                stat.TotalDamageDealtAvgPerMin /= count;
                stat.GoldEarnedAvgPerMin /= count;
                stat.MinionsKilledAvgPerMin /= count;
                stat.FirstBloodParticipationPercent = (stat.FirstBloodParticipationPercent / count) * 100.0f;

                result.Add(stat);
            }
            if (CachingEnabled) cache.Store(result, SummonerUniqueName, ChampionOnLateStatCacheSubdirectoryName);
            return result;
        }

        public Dictionary<string, string> GetSummonerData()
        {
            SummonerDTO summoner = null;
            bool loadedFromCache = true;
            if (CachingEnabled) summoner = cache.Load<SummonerDTO>(SummonerUniqueName, SummonerDataCacheSubdirectoryName);
            if (summoner == null)
            {
                loadedFromCache = false;
                summoner = RiotApiController.GetSummonerByName(SummonerName);
            }

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

            if (CachingEnabled && !loadedFromCache) cache.Store(summoner, SummonerUniqueName, SummonerDataCacheSubdirectoryName);

            return resultMap;                                   
        }

        public List<Dictionary<string, string>> GetLeagueEntryData()
        {
            List<LeagueEntryDTO> leagueEntryList = null;
            bool loadedFromCache = true;
            if (CachingEnabled) leagueEntryList = cache.Load<List<LeagueEntryDTO>>(SummonerUniqueName, LeagueEntriesCacheSubdirectoryName);
            if (leagueEntryList == null)
            {
                loadedFromCache = false;
                leagueEntryList = RiotApiController.GetEntryBySummoner(SummonerId);
            }

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

            if (CachingEnabled && !loadedFromCache) cache.Store(leagueEntryList, SummonerUniqueName, LeagueEntriesCacheSubdirectoryName);

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
