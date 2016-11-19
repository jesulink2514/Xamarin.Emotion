using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;

namespace Emotion.Services
{
    public class EmotionService
    {
        private static async Task<Microsoft.ProjectOxford.Emotion.Contract.Emotion[]> GetHappinessAsync(Stream stream)
        {
            var emotionClient = new EmotionServiceClient("");

            var emotionResults = await emotionClient.RecognizeAsync(stream);

            if (emotionResults == null || !emotionResults.Any())
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        //Average happiness calculation in case of multiple people
        public static async Task<float> GetAverageHappinessScoreAsync(Stream stream)
        {
            Microsoft.ProjectOxford.Emotion.Contract.Emotion[] emotionResults = await GetHappinessAsync(stream);

            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }

            return score / emotionResults.Count();
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            var result = Math.Round(score, 2);

            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }
}
