Feature: OAuth Login
	In order to log in to Twitter
	As a user
	I want to log in using OAuth

@oauth
Scenario: Get authorization URL
	Given a new OAuth login dialog
	And the Oauth service returns an URL
	When I have pressed the Authorize button
	Then the system should retrieve the authorization URL
	And launch it in the browser

@oauth
Scenario: Show pin
	Given a new OAuth login dialog
	And the Oauth service returns an URL
	When I have pressed the Authorize button
	Then the system should display textbox for inputting pin

