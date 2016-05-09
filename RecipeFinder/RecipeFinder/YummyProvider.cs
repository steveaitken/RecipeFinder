using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YummyProvider
{
    public abstract class Metadata
    {
        public string searchValue;

        public abstract bool IsMatch(string term);
	}

	public class MetadataIngredient:Metadata
	{
		public string description;
		public string term;

        public override bool IsMatch(string term)
        {
            return this.term.Equals(term, StringComparison.InvariantCultureIgnoreCase);
        }
    }

	public class MetadataAllergy:Metadata
	{
		public string id;
		public string shortDescription;
		public string longDescription;
		public string type;
		public string[] localesAvailableIn;

        public override bool IsMatch(string term)
        {
            return shortDescription.Equals(term, StringComparison.InvariantCultureIgnoreCase);
        }
    }

	public class MetadataDiet:Metadata
	{
		public string id;
		public string shortDescription;
		public string longDescription;
		public string type;
		public string[] localesAvailableIn;

        public override bool IsMatch(string term)
        {
            return shortDescription.Equals(term, StringComparison.InvariantCultureIgnoreCase);
        }
    }

	public class MetadataCuisine:Metadata
	{
		public string id;
		public string name;
		public string type;
		public string description;
		public string[] localesAvailableIn;

        public override bool IsMatch(string term)
        {
            return description.Equals(term, StringComparison.InvariantCultureIgnoreCase);
        }
    }

	public class MetadataCourse:Metadata
	{
		public string id;
		public string name;
		public string type;
		public string description;
		public string[] localesAvailableIn;

        public override bool IsMatch(string term)
        {
            return description.Equals(term, StringComparison.InvariantCultureIgnoreCase);
        }
    }

	public enum MetadataDictionaryType
    {
		None,
		Ingredient,
		Allergy,
		Diet,
		Cuisine,
		Course,
	}







    public class MetadataDictionary
    {
		public MetadataDictionaryType metadataDictionaryType;
		public string requestURL;
		public Metadata[] metadataDictionary;
		public Type metadataResponseType;

		public MetadataDictionary(MetadataDictionaryType metadataDictionaryType, string requestURL, Type metadataResponseType)
		{
			this.requestURL = requestURL;
			this.metadataDictionaryType = metadataDictionaryType;
			this.metadataResponseType = metadataResponseType;
		}

	}

	public enum SearchParameterType
	{
		AllowedIngredient,
		ExcludedIngredient,
		AllowedAllergy,
		AllowedDiet,
		AllowedCuisine,
		ExcludedCuisine,
		AllowedCourse,
		ExcludedCourse,
		Search,
		MaxResult,
		Start,
		MaxTotalTimeInSeconds
	}

	public struct SearchParameter
	{
		public SearchParameterType searchParameterType;
		public string URLParameter;
        public MetadataDictionaryType metadataDictionaryType;

        public SearchParameter(SearchParameterType searchParameterType, string URLParameter, MetadataDictionaryType metadataDictionaryType)
		{
			this.searchParameterType = searchParameterType;
			this.URLParameter = URLParameter;
            this.metadataDictionaryType = metadataDictionaryType;
		}

	}

	public class YummyRequest
	{
		// assumed AND operation between
		public YummyRequestCondition[] yummyRequestCondition;

        public YummyRequest(YummyRequestCondition[] yummyRequestCondition)
        {
            this.yummyRequestCondition = yummyRequestCondition;
        }
    }

	public struct YummyRequestCondition
	{
		public SearchParameterType searchParameterType;

		// for SearchParameterType.Search it should be general text of search - name of the product
		// for SearchParameterType.MaxResult it should be a number

		public string condition;

        public YummyRequestCondition(SearchParameterType searchParameterType, string condition)
        {
            this.searchParameterType = searchParameterType;
            this.condition = condition;
        }
    }



	public class Criteria
	{
		public string q;
		public object allowedIngredient;
		public object excludedIngredient;
	}

	public class ImageUrlsBySize
	{
		public string __invalid_name__90;
		public string __invalid_name__360 { get; set; }
	}

	public class Attributes
	{
		public string[] course;
	}

	public class Flavors
	{
		public double piquant;
		public double meaty;
		public double bitter;
		public double sweet;
		public double sour;
		public double salty;


		public override string ToString()
		{
			return null;
		}
	}

	public class Match
	{
		//public ImageUrlsBySize imageUrlsBySize;
		public string sourceDisplayName;
		public string[] ingredients;
		public string id;
		public string[] smallImageUrls;
		public string recipeName;
		public int totalTimeInSeconds;
		public Attributes attributes;
		public Flavors flavors;
		public int rating;
	}

	public class YummyRecipesResponse
	{
		public Criteria criteria;
		public Match[] matches;
		public int totalMatchCount;
	}






	public class Image
	{
		public string hostedSmallUrl { get; set; }
		public string hostedMediumUrl { get; set; }
		public string hostedLargeUrl { get; set; }
		public ImageUrlsBySize imageUrlsBySize { get; set; }
	}

	public class Source
	{
		public string sourceDisplayName { get; set; }
		public string sourceSiteUrl { get; set; }
		public string sourceRecipeUrl { get; set; }
	}


	public class NutritionEstimate
	{
		public string attribute { get; set; }
		public string description { get; set; }
		public double value { get; set; }
		public Unit unit { get; set; }
	}

	public class Unit
	{
		public string id { get; set; }
		public string name { get; set; }
		public string abbreviation { get; set; }
		public string plural { get; set; }
		public string pluralAbbreviation { get; set; }
		public bool @decimal { get; set; }
	}

	public class YummyRecipeResponse
	{
		public object yield;
		public int prepTimeInSeconds;
		public NutritionEstimate[] nutritionEstimates;
		public string totalTime;
		public Image[] images;
		public string name;
		public Source source;
		public string prepTime;
		public string id;
		public string[] ingredientLines;
		public int numberOfServings;
		public int totalTimeInSeconds;
		public Attributes attributes;
		public Flavors flavors;
		public int rating;
	}





	public class YummlyProvider
    {
        static string rootURL = "https://api.yummly.com/v1/api";
		static string applicationID = "ade40f2d";
		static string applicationKey = "2c70977d7f3ce47644cb2e8e69d71409";
		static string idKey = "_app_id=" + applicationID +"&_app_key=" + applicationKey;

		static string recipesURL = "/recipes";
		static string recipeURL = "/recipe";

		static MetadataDictionary[] metadataDictionaries = new MetadataDictionary[]
		{
			new MetadataDictionary(MetadataDictionaryType.Ingredient, "/metadata/ingredient", typeof(MetadataIngredient[])),
			new MetadataDictionary(MetadataDictionaryType.Allergy, "/metadata/allergy", typeof(MetadataAllergy[])),
			new MetadataDictionary(MetadataDictionaryType.Diet, "/metadata/diet", typeof(MetadataDiet[])),
			new MetadataDictionary(MetadataDictionaryType.Cuisine, "/metadata/cuisine", typeof(MetadataCuisine[])),
			new MetadataDictionary(MetadataDictionaryType.Course, "/metadata/course", typeof(MetadataCourse[])),
		};

		static SearchParameter[] searchParameters = new SearchParameter[]
		{
			new SearchParameter(SearchParameterType.AllowedIngredient, "allowedIngredient[]", MetadataDictionaryType.Ingredient),
			new SearchParameter(SearchParameterType.ExcludedIngredient, "excludedIngredient[]", MetadataDictionaryType.Ingredient),
			new SearchParameter(SearchParameterType.AllowedAllergy, "allowedAllergy[]", MetadataDictionaryType.Allergy),
			new SearchParameter(SearchParameterType.AllowedDiet, "allowedDiet[]", MetadataDictionaryType.Diet),
			new SearchParameter(SearchParameterType.AllowedCuisine, "allowedCuisine[]", MetadataDictionaryType.Cuisine),
			new SearchParameter(SearchParameterType.ExcludedCuisine, "excludedCuisine[]", MetadataDictionaryType.Cuisine),
			new SearchParameter(SearchParameterType.AllowedCourse, "allowedCourse[]", MetadataDictionaryType.Course),
			new SearchParameter(SearchParameterType.ExcludedCourse, "excludedCourse[]", MetadataDictionaryType.Course),
			new SearchParameter(SearchParameterType.Search, "q", MetadataDictionaryType.None),
			new SearchParameter(SearchParameterType.MaxResult, "maxResult", MetadataDictionaryType.None),
			new SearchParameter(SearchParameterType.MaxTotalTimeInSeconds, "maxTotalTimeInSeconds", MetadataDictionaryType.None),
		};

        public static Metadata[] GetMetadata(MetadataDictionaryType metadataDictionaryType)
        {
            return metadataDictionaries.First(md => md.metadataDictionaryType == metadataDictionaryType).metadataDictionary;
        }

		public YummlyProvider()
		{
			GetMetadata1();
		}

		void GetMetadata()
		{
			HttpClient hc = new HttpClient();

			for (int i = 0; i < metadataDictionaries.Length; ++i)
			{
				string query = rootURL + metadataDictionaries[i].requestURL + "?" + idKey;

				Task<HttpResponseMessage> t = hc.GetAsync(query);
				t.Wait();

				Task<string> t1 = t.Result.Content.ReadAsStringAsync();
				t1.Wait();

				int ind1 = t1.Result.IndexOf('[');
				int ind2 = t1.Result.LastIndexOf(']');

				string res1 = t1.Result.Substring(ind1, ind2 - ind1 + 1);

				metadataDictionaries[i].metadataDictionary = (Metadata[])JsonConvert.DeserializeObject(res1, metadataDictionaries[i].metadataResponseType);

				/*FileStream f = File.Create("md_" + i.ToString());
				StreamWriter sw = new StreamWriter(f);
				sw.Write(res1);
				sw.Flush();
				f.Flush();
				f.Close();*/
			}
		}


        public static MetadataDictionaryType GetMetadataDictionaryType(SearchParameterType searchParameterType)
        {
            MetadataDictionaryType result = MetadataDictionaryType.None;

            if (searchParameters.Any(sp => sp.searchParameterType == searchParameterType))
                result = searchParameters.First(sp => sp.searchParameterType == searchParameterType).metadataDictionaryType;

            return result;
        }




        void GetMetadata1()
		{
			for (int i = 0; i < metadataDictionaries.Length; ++i)
			{
				FileStream f = File.OpenRead(System.Web.HttpContext.Current.Server.MapPath("~/").TrimEnd('\\') + "\\YummyMetaData\\md_" + i.ToString() + ".json");
				StreamReader sr = new StreamReader(f);
				string res1 = sr.ReadToEnd();
				f.Close();

				metadataDictionaries[i].metadataDictionary = (Metadata[])JsonConvert.DeserializeObject(res1, metadataDictionaries[i].metadataResponseType);
			}
		}

		string CreateGetRecipesQuery(YummyRequest yummyRequest)
		{
			string query = rootURL + recipesURL +  "?" + idKey;

            for (int i = 0; i < yummyRequest.yummyRequestCondition.Length; ++i)
            {
                bool add = true, addFilter = false;
                string filter = null;

                MetadataDictionaryType mdt = GetMetadataDictionaryType(yummyRequest.yummyRequestCondition[i].searchParameterType);

                if (mdt != MetadataDictionaryType.None)
                {
                    MetadataDictionary md = metadataDictionaries.FirstOrDefault(md1 => md1.metadataDictionaryType == mdt);
                    if (md != null)
                    {
                        add = md.metadataDictionary.Select(md1 => md1.IsMatch(yummyRequest.yummyRequestCondition[i].condition)).Count() > 0;
                        addFilter = true;
                        filter = md.metadataDictionary.First(md1 => md1.IsMatch(yummyRequest.yummyRequestCondition[i].condition)).searchValue;
                    }
                }

                if (add)
				{
					string h = "&";
					h += searchParameters.First(sp => sp.searchParameterType == yummyRequest.yummyRequestCondition[i].searchParameterType).URLParameter;

                    if (!addFilter)
                        h += "=" + yummyRequest.yummyRequestCondition[i].condition;
                    else
                        h += "=" + filter;

                    query += h;
				}
			}

			return query;
		}

		public async Task<YummyRecipesResponse> GetRecipes(YummyRequest yummyRequest)
		{
			string query = CreateGetRecipesQuery(yummyRequest);

			HttpClient hc = new HttpClient();

			HttpResponseMessage t = await hc.GetAsync(query);
			string t1 = await t.Content.ReadAsStringAsync();

			if (!string.IsNullOrWhiteSpace(t1))
				return (YummyRecipesResponse)JsonConvert.DeserializeObject(t1, typeof(YummyRecipesResponse));
			else
				return null;
		}


		public async Task<YummyRecipeResponse> GetRecipe(string recipeID)
		{
			string query = rootURL + recipeURL + "/" + recipeID + "?" + idKey;

			HttpClient hc = new HttpClient();

			HttpResponseMessage t = await hc.GetAsync(query);
			string t1 = await t.Content.ReadAsStringAsync();

			if (!string.IsNullOrWhiteSpace(t1))
				return (YummyRecipeResponse)JsonConvert.DeserializeObject(t1, typeof(YummyRecipeResponse));
			else
				return null;
		}
	}
}