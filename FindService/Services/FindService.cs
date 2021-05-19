using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcFind;
using GrpcText;
using Microsoft.AspNetCore.Authorization;

namespace FindService.Services
{
    [Authorize]
    public class FindService : Find.FindBase
    {

        private readonly Text.TextClient _textClient;

        public FindService(Text.TextClient textClient)
        {
            _textClient = textClient;
        }

        public override async Task<FindResponse> FindWords(FindRequest request, ServerCallContext context)
        {
            var response = new FindResponse();
            var textRequest = new TextRequest { Id = request.TextId };
            var textResponse = await _textClient.GetTextByIdAsync(textRequest);
            var text = textResponse.Body;
            var wordsForSearch = request.SearchWords;
            foreach (var wordForSearch in wordsForSearch)
            {
                if (Regex.IsMatch(text, $"\\b{wordForSearch}\\b"))
                {
                    response.FoundWords.Add(wordForSearch);
                }
            }
            return response;
        }
    }
}
