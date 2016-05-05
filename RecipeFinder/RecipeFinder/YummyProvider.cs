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
    public class MetadataResponse
    {
        public string searchValue;
	}

	public class MetadataResponseIngredient:MetadataResponse
	{
		public string description;
		public string term;
	}

	public class MetadataResponseAllergy:MetadataResponse
	{
		public string id;
		public string shortDescription;
		public string longDescription;
		public string type;
		public string[] localesAvailableIn;
	}

	public class MetadataResponseDiet:MetadataResponse
	{
		public string id;
		public string shortDescription;
		public string longDescription;
		public string type;
		public string[] localesAvailableIn;
	}

	public class MetadataResponseCuisine:MetadataResponse
	{
		public string id;
		public string name;
		public string type;
		public string description;
		public string[] localesAvailableIn;
	}

	public class MetadataResponseCourse:MetadataResponse
	{
		public string id;
		public string name;
		public string type;
		public string description;
		public string[] localesAvailableIn;
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
		public MetadataResponse[] metadataDictionary;
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

		public SearchParameter(SearchParameterType searchParameterType, string URLParameter)
		{
			this.searchParameterType = searchParameterType;
			this.URLParameter = URLParameter;
		}

	}

	public class YummyRequest
	{
		// assumed AND operation between
		public YummyRequestCondition[] yummyRequestCondition;
	}

	public class YummyRequestCondition
	{
		public MetadataDictionaryType metadataDictionaryType;
		public SearchParameterType searchParameterType;

		// for SearchParameterType.Search it should be general text of search - name of the product
		// for SearchParameterType.MaxResult it should be a number

		public string condition;
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
        static string rootURL = "http://api.yummly.com/v1/api";
		static string applicationID = "ade40f2d";
		static string applicationKey = "2c70977d7f3ce47644cb2e8e69d71409";
		static string idKey = "_app_id=" + applicationID +"&_app_key=" + applicationKey;

		static string recipesURL = "/recipes";
		static string recipeURL = "/recipe";

		static MetadataDictionary[] metadataDictionaries = new MetadataDictionary[]
		{
			new MetadataDictionary(MetadataDictionaryType.Ingredient, "/metadata/ingredient", typeof(MetadataResponseIngredient[])),
			new MetadataDictionary(MetadataDictionaryType.Allergy, "/metadata/allergy", typeof(MetadataResponseAllergy[])),
			new MetadataDictionary(MetadataDictionaryType.Diet, "/metadata/diet", typeof(MetadataResponseDiet[])),
			new MetadataDictionary(MetadataDictionaryType.Cuisine, "/metadata/cuisine", typeof(MetadataResponseCuisine[])),
			new MetadataDictionary(MetadataDictionaryType.Course, "/metadata/course", typeof(MetadataResponseCourse[])),
		};

		static SearchParameter[] searchParameters = new SearchParameter[]
			{
				new SearchParameter(SearchParameterType.AllowedIngredient, "allowedIngredient[]"),
				new SearchParameter(SearchParameterType.ExcludedIngredient, "excludedIngredient[]"),
				new SearchParameter(SearchParameterType.AllowedAllergy, "allowedAllergy[]"),
				new SearchParameter(SearchParameterType.AllowedDiet, "allowedDiet[]"),
				new SearchParameter(SearchParameterType.AllowedCuisine, "allowedCuisine[]"),
				new SearchParameter(SearchParameterType.ExcludedCuisine, "excludedCuisine[]"),
				new SearchParameter(SearchParameterType.AllowedCourse, "allowedCourse[]"),
				new SearchParameter(SearchParameterType.ExcludedCourse, "excludedCourse[]"),
				new SearchParameter(SearchParameterType.Search, "q"),
				new SearchParameter(SearchParameterType.MaxResult, "maxResult"),
				new SearchParameter(SearchParameterType.MaxTotalTimeInSeconds, "maxTotalTimeInSeconds"),
			};

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

				metadataDictionaries[i].metadataDictionary = (MetadataResponse[])JsonConvert.DeserializeObject(res1, metadataDictionaries[i].metadataResponseType);

				/*FileStream f = File.Create("md_" + i.ToString());
				StreamWriter sw = new StreamWriter(f);
				sw.Write(res1);
				sw.Flush();
				f.Flush();
				f.Close();*/
			}
		}

		void GetMetadata1()
		{
			for (int i = 0; i < metadataDictionaries.Length; ++i)
			{
				FileStream f = File.OpenRead("YummyMetaData\\md_" + i.ToString());
				StreamReader sr = new StreamReader(f);
				string res1 = sr.ReadToEnd();
				f.Close();

				metadataDictionaries[i].metadataDictionary = (MetadataResponse[])JsonConvert.DeserializeObject(res1, metadataDictionaries[i].metadataResponseType);
			}
		}

		public string CreateGetRecipesQuery(YummyRequest yummyRequest)
		{
			string query = rootURL + recipesURL +  "?" + idKey;

			for (int i = 0; i < yummyRequest.yummyRequestCondition.Length; ++i)
			{
				bool add = true;

				if (yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.AllowedAllergy
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.AllowedCourse
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.AllowedCuisine
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.AllowedDiet
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.AllowedIngredient
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.ExcludedCourse
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.ExcludedCuisine
					|| yummyRequest.yummyRequestCondition[i].searchParameterType == SearchParameterType.ExcludedIngredient)
				{
					MetadataDictionary md = metadataDictionaries.First(md1 => md1.metadataDictionaryType == yummyRequest.yummyRequestCondition[i].metadataDictionaryType);

					if (md != null)
						if (md.metadataDictionary.Select(md1 => md1.searchValue == yummyRequest.yummyRequestCondition[i].condition).Count() == 0)
							add = false;
				}

				if (add)
				{
					string h = "&";
					h += searchParameters.First(sp => sp.searchParameterType == yummyRequest.yummyRequestCondition[i].searchParameterType).URLParameter;
					h += "=" + yummyRequest.yummyRequestCondition[i].condition;

					query += h;
				}
			}

			return query;
		}


		public YummyRecipesResponse GetRecipes(YummyRequest yummyRequest)
		{
			string query = CreateGetRecipesQuery(yummyRequest);

			HttpClient hc = new HttpClient();

			Task<HttpResponseMessage> t = hc.GetAsync(query);
			t.Wait();

			Task<string> t1 = t.Result.Content.ReadAsStringAsync();
			t1.Wait();

			if (!string.IsNullOrWhiteSpace(t1.Result))
				return (YummyRecipesResponse)JsonConvert.DeserializeObject(t1.Result, typeof(YummyRecipesResponse));
			else
				return null;
		}


		public YummyRecipeResponse GetRecipe(string recipeID)
		{
			string query = rootURL + recipeURL + "/" + recipeID + "?" + idKey;

			HttpClient hc = new HttpClient();

			Task<HttpResponseMessage> t = hc.GetAsync(query);
			t.Wait();

			Task<string> t1 = t.Result.Content.ReadAsStringAsync();
			t1.Wait();

			if (!string.IsNullOrWhiteSpace(t1.Result))
				return (YummyRecipeResponse)JsonConvert.DeserializeObject(t1.Result, typeof(YummyRecipeResponse));
			else
				return null;
		}
	}
}