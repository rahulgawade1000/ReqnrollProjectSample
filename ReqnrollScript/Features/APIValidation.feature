Feature: API Response Validation Against Aerospike

@mytag
	Scenario Outline: Validate API response matches Aerospike dataset
		Given I have an API endpoint "<apiUrl>"
		When I fetch data for user "<userId>"
		Then the API response should match the Aerospike dataset for "<userId>"

		Examples:
		  | apiUrl                        | userId |
		  | https://api.example.com/users | 101    |
		  | https://api.example.com/users | 102    |
		  | https://api.example.com/users | 103    |
