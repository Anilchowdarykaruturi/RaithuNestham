
namespace RaithuNestham.Services;

public class WeatherAdvisoryService
{
    public object GetCropAdvice(
        string cropName,
        string growthStage,
        double rainProbability,
        double rainfallMm,
        double windSpeedKmh)
    {
        var crop = cropName.Trim().ToLowerInvariant();
        var stage = growthStage.Trim().ToLowerInvariant();

        var isHarvested =
            stage.Contains("harvest") ||
            stage.Contains("కోత") ||
            stage.Contains("harvested");

        var paddyCrops = new[]
        {
            "paddy", "rice", "వరి", "బియ్యం"
        };

        var isPaddy = paddyCrops.Contains(crop);

        string harvestAdvice;
        if (isHarvested && (rainProbability >= 50 || rainfallMm >= 1))
        {
            harvestAdvice =
                "రేపు వర్షం పడే అవకాశం ఉంది. " +
                "కోసిన పంట లేదా ధాన్యం తడవకుండా కాపాడుకోండి.";
        }
        else if (isHarvested)
        {
            harvestAdvice =
                "రేపు వర్ష సూచన తక్కువగా ఉండవచ్చు. " +
                "కోత చేసిన పంటను జాగ్రత్తగా నిర్వహించండి.";
        }
        else if (rainProbability >= 70 || rainfallMm >= 10)
        {
            harvestAdvice =
                "రేపు వర్షం పడే అవకాశం ఉంది. " +
                "పంట కోతకు సిద్ధంగా ఉంటే వాతావరణాన్ని బట్టి పనులను ప్లాన్ చేయండి.";
        }
        else
        {
            harvestAdvice =
                "రేపటి వాతావరణాన్ని బట్టి పంట పనులను ప్లాన్ చేసుకోండి.";
        }

        string fertilizerAdvice;
        if (rainProbability >= 70 || rainfallMm >= 10)
        {
            fertilizerAdvice =
                "రేపు భారీ వర్షం పడే అవకాశం ఉంది. " +
                "ఎరువులు వేయడం వాయిదా వేయండి.";
        }
        else if (rainProbability >= 40 || rainfallMm >= 5)
        {
            fertilizerAdvice =
                "రేపు వర్షం పడే అవకాశం ఉంది. " +
                "ఎరువులు వేసే సమయాన్ని వర్ష సూచనను బట్టి నిర్ణయించండి.";
        }
        else
        {
            fertilizerAdvice =
                "రేపు వర్ష సూచన తక్కువగా ఉండవచ్చు. " +
                "పంటకు ఎరువులు అవసరమైన దశలో ఉంటే వేయవచ్చు.";
        }

        string pesticideAdvice;
        if (rainProbability >= 50 || rainfallMm >= 1)
        {
            pesticideAdvice =
                "రేపు వర్షం పడే అవకాశం ఉంది. " +
                "రేపు పురుగుమందులు పిచికారీ చేయవద్దు.";
        }
        else if (windSpeedKmh >= 20)
        {
            pesticideAdvice =
                "రేపు బలమైన గాలులు వీచే అవకాశం ఉంది. " +
                "రేపు పురుగుమందులు పిచికారీ చేయవద్దు.";
        }
        else
        {
            pesticideAdvice =
                "రేపు వాతావరణం అనుకూలంగా ఉండవచ్చు. " +
                "పంటకు పురుగుమందులు అవసరమైతే మాత్రమే పిచికారీ చేయండి.";
        }

        string cropDisplayName = GetCropDisplayName(cropName);

        return new
        {
            cropName = cropDisplayName,
            growthStage,
            rainProbabilityPercent = rainProbability,
            expectedRainfallMm = rainfallMm,
            maximumWindSpeedKmh = windSpeedKmh,
            harvestAdvice,
            fertilizerAdvice,
            pesticideAdvice
        };
    }

    private static string GetCropDisplayName(string cropName)
    {
        return cropName.Trim().ToLowerInvariant() switch
        {
            "paddy" or "rice" or "వరి" or "బియ్యం" => "వరి",
            "cotton" or "పత్తి" => "పత్తి",
            "maize" or "corn" or "మొక్కజొన్న" => "మొక్కజొన్న",
            "chilli" or "chili" or "మిరప" => "మిరప",
            "groundnut" or "వేరుశనగ" => "వేరుశనగ",
            "banana" or "అరటి" => "అరటి",
            "sugarcane" or "చెరకు" => "చెరకు",
            "tomato" or "టమాటా" => "టమాటా",
            "turmeric" or "పసుపు" => "పసుపు",
            "mung bean" or "green gram" or "పెసలు" => "పెసలు",
            _ => cropName.Trim()
        };
    }
}
