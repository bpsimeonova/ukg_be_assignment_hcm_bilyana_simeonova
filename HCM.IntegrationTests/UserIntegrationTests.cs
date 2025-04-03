namespace HCM.IntegrationTests
{
	public class UserIntegrationTests
	{
		
		[Fact]
		public async Task GetAllUsers_ShouldReturnExpectedView()
		{
			// Arrange
			var application = new HCMWebApplicationFactory();
			var client = application.CreateClient();

			// Act
			var response = await client.GetAsync("/User/Index");

			// Assert
			response.EnsureSuccessStatusCode();
			var responseHtml = await response.Content.ReadAsStringAsync();

			Assert.Contains("Employee List", responseHtml); // Check if expected view is returned
			Assert.Contains("admin@hcm.com", responseHtml); // Check if expected model data are returned
		}
	}
}