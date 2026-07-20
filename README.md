# OutSurance Test Automation Academy 🎓

Welcome to the **OutSurance Test Automation Academy** — a hands-on learning
repository designed to help aspiring test automation engineers at OutSurance
build real-world skills using industry-standard tools.

---

## Tech Stack

| Tool | Purpose |
|---|---|
| **C# / .NET 8** | Language and runtime |
| **NUnit 3** | Test framework |
| **Microsoft.Playwright.NUnit** | Browser automation |
| **reqres.in** | REST API practice target |

---

## Solution Structure

    OutSurance.TestAutomation.Academy/
    |
    +-- src/
    |   +-- TestAutomation.Core/
    |   |   +-- Base/
    |   |   |   +-- BasePage.cs        # Shared Playwright helpers for all Page Objects
    |   |   |   +-- BaseTest.cs        # NUnit setup/teardown + screenshot on failure
    |   |   +-- Config/
    |   |       +-- TestSettings.cs    # Browser config, base URLs, credentials
    |   |
    |   +-- TestAutomation.PageObjects/
    |       +-- SauceDemo/
    |           +-- LoginPage.cs
    |           +-- ProductsPage.cs
    |           +-- CartPage.cs
    |
    +-- tests/
        +-- TestAutomation.Challenges.UI/
        |   +-- SauceDemo/
        |       +-- Challenge1_LoginTests.cs   Beginner to Advanced
        |       +-- Challenge2_CartTests.cs    Intermediate to Advanced
        |
        +-- TestAutomation.Challenges.API/
            +-- (coming soon)

---

## Prerequisites

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with the C# extension
3. Run the Playwright browser installer **once** after cloning:

<!-- -->

    cd tests/TestAutomation.Challenges.UI
    dotnet build
    pwsh bin/Debug/net8.0/playwright.ps1 install

---

## Running the Tests

### Run all tests

    dotnet test

### Run a specific challenge

    dotnet test --filter "Category=Login"
    dotnet test --filter "Category=Cart"

### Run in headed mode (see the browser)

    $env:TEST_HEADLESS="false"; dotnet test

### Run with slow-motion (great for demos)

    $env:TEST_HEADLESS="false"; $env:TEST_SLOW_MO="500"; dotnet test

---

## Configuration via Environment Variables

All settings can be overridden without changing source code.
Variable names match the property names in `testsettings.json` exactly.

| Variable | Default | Description |
|---|---|---|
| `Browser` | `chromium` | `chromium`, `firefox`, or `webkit` |
| `Headless` | `true` | `true` or `false` |
| `SlowMo` | `0` | Milliseconds between actions |
| `DefaultTimeoutMs` | `30000` | Default action timeout (ms) |
| `SauceDemoBaseUrl` | `https://www.saucedemo.com` | Override for proxies/mirrors |
| `SauceDemoStandardUser` | `standard_user` | Override default username |
| `SauceDemoPassword` | `secret_sauce` | Override default password |
| `REQRES_API_KEY` | *(none)* | Required for all API challenges — contact the test automation team to obtain this key |

> **Note:** `REQRES_API_KEY` is the only value that does **not** belong in `testsettings.json`.
> It must be set as an environment variable on your local machine.
> Do not attempt to generate your own key — contact the test automation team.

---

## Practice Websites

| Site | URL | Focus Area |
|---|---|---|
| SauceDemo | https://www.saucedemo.com | Login, cart, checkout |
| The Internet | https://the-internet.herokuapp.com | Broad UI challenges |
| DemoQA | https://demoqa.com | Forms and widgets |
| ReqRes | https://reqres.in | REST API testing |

---

## The Challenges

### Challenge 1 - Login (Challenge1_LoginTests.cs)

Practise navigating to a page, filling in forms, clicking buttons, and
asserting on the result.

| # | Test | Tier |
|---|---|---|
| 1 | Valid credentials redirect to products page | Beginner |
| 2 | Invalid password shows error message | Beginner |
| 3 | Empty username shows validation error | Beginner |
| 4 | Locked-out user sees locked-out error | Intermediate |
| 5 | Empty password shows validation error | Intermediate |
| 6 | All valid user accounts can log in | Advanced |

---

### Challenge 2 - Shopping Cart (Challenge2_CartTests.cs)

Practise multi-step user journeys, dynamic UI assertions, and data-driven tests.

| # | Test | Tier |
|---|---|---|
| 1 | Adding one product updates badge to 1 | Beginner |
| 2 | Correct product appears in cart after add | Beginner |
| 3 | Multiple products show correct badge and cart | Intermediate |
| 4 | Removing one product from multi-item cart | Intermediate |
| 5 | Continue Shopping returns to products page | Intermediate |
| 6 | Cart persists contents after navigating away | Advanced |
| 7 | Data-driven: each product can be added and removed | Advanced |

---

## How to Complete a Challenge

1. Open the relevant challenge file in `tests/TestAutomation.Challenges.UI/`.
2. Read the XML doc comment above each test — it describes the steps and acceptance criteria.
3. Replace `throw new NotImplementedException("Complete this challenge!");` with your implementation.
4. Run the specific test to verify your solution:

<!-- -->

    dotnet test --filter "FullyQualifiedName~Challenge1"

5. All assertions must use NUnit constraint syntax:

<!-- -->

    Assert.That(actualValue, Is.EqualTo(expectedValue), "Meaningful failure message");

---

## Framework Rules (Read Before Writing Tests)

- Use Page Object methods — never access `Page` directly from a test.
- Use Playwright built-in auto-waiting — no `Thread.Sleep` or `Task.Delay`.
- Each test must be independently runnable in any order.
- All assertions must use `Assert.That(...)` with a descriptive failure message.
- Do not hard-code URLs, usernames, or passwords — use `Settings.*` properties.
- Do not share mutable state between tests.

---

## Screenshots on Failure

When a test fails, a full-page screenshot is automatically saved to:

    <TestOutputDirectory>/screenshots/<TestName>_<Timestamp>.png

The path is also attached to the NUnit test result and printed to the console.

---

## Need Help?

- Playwright docs: https://playwright.dev/dotnet/
- NUnit docs: https://docs.nunit.org/
- SauceDemo source hints: https://www.saucedemo.com

Good luck and happy automating!
