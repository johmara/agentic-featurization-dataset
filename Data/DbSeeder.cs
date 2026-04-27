using Microsoft.EntityFrameworkCore;
using ReferenceManager.Models;

namespace ReferenceManager.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Papers.AnyAsync())
        {
            db.Papers.AddRange(
                new Paper
                {
                    Title = "HAnS: IDE-Based Editing Support for Embedded Feature Annotations",
                    Year = 2021,
                    Abstract = "Presents HAnS, an IntelliJ plugin for editing embedded feature annotations in source code, providing features such as code completion, navigation, and consistency checking.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Herman Jansson", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Mukelabai Mukelabai", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Alexandre Bergel", Email = null, Affiliations = [new() { Name = "University of Chile", City = "Santiago", Country = "Chile" }] },
                        new() { Name = "Truong Ho-Quang", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                    ]
                },
                new Paper
                {
                    Title = "What Is a Feature? A Qualitative Study of Features in Industrial Software Product Lines",
                    Year = 2015,
                    Abstract = "Reports a qualitative study investigating what practitioners consider to be features in industrial software product lines across multiple companies.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Daniela Lettner", Email = null, Affiliations = [new() { Name = "Johannes Kepler University Linz", City = "Linz", Country = "Austria" }] },
                        new() { Name = "Julia Rubin", Email = null, Affiliations = [new() { Name = "University of British Columbia", City = "Vancouver", Country = "Canada" }] },
                        new() { Name = "Paul Grünbacher", Email = null, Affiliations = [new() { Name = "Johannes Kepler University Linz", City = "Linz", Country = "Austria" }] },
                        new() { Name = "Adeline Silva", Email = null, Affiliations = [] },
                        new() { Name = "Martin Becker", Email = null, Affiliations = [] },
                        new() { Name = "Marsha Chechik", Email = null, Affiliations = [new() { Name = "University of Toronto", City = "Toronto", Country = "Canada" }] },
                        new() { Name = "Krzysztof Czarnecki", Email = null, Affiliations = [new() { Name = "University of Waterloo", City = "Waterloo", Country = "Canada" }] },
                    ]
                },
                new Paper
                {
                    Title = "Variability Mining with LEADT",
                    Year = 2011,
                    Abstract = "Presents LEADT, a tool for mining feature-to-code mappings from preprocessor-based software product lines.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Christian Kästner", Email = null, Affiliations = [new() { Name = "Philipps-Universität Marburg", City = "Marburg", Country = "Germany" }] },
                        new() { Name = "Alexander Dreiling", Email = null, Affiliations = [new() { Name = "Philipps-Universität Marburg", City = "Marburg", Country = "Germany" }] },
                        new() { Name = "Klaus Ostermann", Email = null, Affiliations = [new() { Name = "Philipps-Universität Marburg", City = "Marburg", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "Feature-Oriented Software Product Lines: Concepts and Implementation",
                    Year = 2013,
                    Abstract = "A comprehensive textbook covering the theory and practice of feature-oriented software product lines, including feature modeling, variability implementation, and product derivation.",
                    Doi = "10.1007/978-3-642-37521-7",
                    Authors =
                    [
                        new() { Name = "Sven Apel", Email = null, Affiliations = [new() { Name = "University of Passau", City = "Passau", Country = "Germany" }] },
                        new() { Name = "Don Batory", Email = null, Affiliations = [new() { Name = "University of Texas at Austin", City = "Austin", Country = "USA" }] },
                        new() { Name = "Christian Kästner", Email = null, Affiliations = [new() { Name = "Carnegie Mellon University", City = "Pittsburgh", Country = "USA" }] },
                        new() { Name = "Gunter Saake", Email = null, Affiliations = [new() { Name = "University of Magdeburg", City = "Magdeburg", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "Feature-Oriented Domain Analysis (FODA) Feasibility Study",
                    Year = 1990,
                    Abstract = "Introduces Feature-Oriented Domain Analysis, a method for identifying and organizing the features of software systems in a domain to support reuse.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Kyo Kang", Email = null, Affiliations = [new() { Name = "Software Engineering Institute", City = "Pittsburgh", Country = "USA" }, new() { Name = "Carnegie Mellon University", City = "Pittsburgh", Country = "USA" }] },
                        new() { Name = "Sholom Cohen", Email = null, Affiliations = [new() { Name = "Software Engineering Institute", City = "Pittsburgh", Country = "USA" }] },
                        new() { Name = "James Hess", Email = null, Affiliations = [new() { Name = "Software Engineering Institute", City = "Pittsburgh", Country = "USA" }] },
                        new() { Name = "William Nowak", Email = null, Affiliations = [new() { Name = "Software Engineering Institute", City = "Pittsburgh", Country = "USA" }] },
                        new() { Name = "Spencer Peterson", Email = null, Affiliations = [new() { Name = "Software Engineering Institute", City = "Pittsburgh", Country = "USA" }] },
                    ]
                },
                new Paper
                {
                    Title = "FeatureIDE: An Extensible Framework for Feature-Oriented Software Development",
                    Year = 2014,
                    Abstract = "Presents FeatureIDE, an Eclipse-based framework that supports all phases of feature-oriented software development, from domain analysis to product derivation.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Thomas Thüm", Email = null, Affiliations = [new() { Name = "University of Magdeburg", City = "Magdeburg", Country = "Germany" }] },
                        new() { Name = "Christian Kästner", Email = null, Affiliations = [new() { Name = "Carnegie Mellon University", City = "Pittsburgh", Country = "USA" }] },
                        new() { Name = "Fabian Benduhn", Email = null, Affiliations = [new() { Name = "University of Magdeburg", City = "Magdeburg", Country = "Germany" }] },
                        new() { Name = "Jens Meinicke", Email = null, Affiliations = [new() { Name = "University of Magdeburg", City = "Magdeburg", Country = "Germany" }] },
                        new() { Name = "Gunter Saake", Email = null, Affiliations = [new() { Name = "University of Magdeburg", City = "Magdeburg", Country = "Germany" }] },
                        new() { Name = "Thomas Leich", Email = null, Affiliations = [new() { Name = "Metop GmbH", City = "Magdeburg", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "FM-PRO: A Feature Modeling Process",
                    Year = 2025,
                    Abstract = "Presents FM-PRO, an empirically grounded process for feature modeling in software product lines, evaluated through a multi-year industrial case study.",
                    Doi = "10.1109/TSE.2024.3513635",
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Wardah Mahmood", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Jude Gyimah", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "FeatRacer: Locating Features Through Assisted Traceability",
                    Year = 2023,
                    Abstract = "Presents FeatRacer, an approach and tool for locating features in source code through assisted traceability, combining automated analysis with human guidance.",
                    Doi = "10.1109/TSE.2023.3290613",
                    Authors =
                    [
                        new() { Name = "Mukelabai Mukelabai", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Kevin Hermann", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Jan-Philipp Steghöfer", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                    ]
                },
                // &begin[Groups]
                new Paper
                {
                    Title = "On Using LLMs to 'Featurize' Software",
                    Year = 2024,
                    Abstract = "Reports exploratory experiments on using large language models to featurize software; defines featurization precisely and determines the potential and limitations of current LLMs for this task.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "An IDE Plugin for Clone Management in Software Product Lines",
                    Year = 2024,
                    Abstract = "Presents an extension of HAnS with clone management capabilities using a lightweight trace database to support synchronization of cloned assets across software product line variants.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Ahmad Al Shihabi", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Jan Sollmann", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Wardah Mahmood", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "Visualizing Feature-Oriented Software Evolution",
                    Year = 2025,
                    Abstract = "Presents a feature evolution timeline IntelliJ plugin that visualizes feature evolution via traceability links and commit history, lifting abstraction to the feature level; validated through three action research cycles and a controlled experiment.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "Lightweight Visualization of Software Features with HAnS-viz",
                    Year = 2025,
                    Abstract = "Presents HAnS-viz, an IntelliJ plugin providing feature-oriented visualizations that build on HAnS embedded annotations to lift code-level assets to feature-level representations.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                    ]
                },
                new Paper
                {
                    Title = "Towards Decentralized Feature Models",
                    Year = 2026,
                    Abstract = "Proposes a framework for decentralized feature modeling; studies core concerns including placement, scope, cross-model dependencies, reuse, and integration through a 15-year systematic literature review.",
                    Doi = null,
                    Authors =
                    [
                        new() { Name = "Johan Martinson", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Wardah Mahmood", Email = null, Affiliations = [new() { Name = "Chalmers University of Technology", City = "Gothenburg", Country = "Sweden" }] },
                        new() { Name = "Mukelabai Mukelabai", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                        new() { Name = "Rafael Capilla", Email = null, Affiliations = [new() { Name = "Rey Juan Carlos University", City = "Madrid", Country = "Spain" }] },
                        new() { Name = "Thorsten Berger", Email = null, Affiliations = [new() { Name = "Ruhr-Universität Bochum", City = "Bochum", Country = "Germany" }] },
                    ]
                }
                // &end[Groups]
            );

            await db.SaveChangesAsync();
        }

        // &begin[Groups]
        if (!await db.Groups.AnyAsync())
        {
            var acceptedTitles = new HashSet<string>
            {
                "HAnS: IDE-Based Editing Support for Embedded Feature Annotations",
                "On Using LLMs to 'Featurize' Software",
                "FM-PRO: A Feature Modeling Process",
                "An IDE Plugin for Clone Management in Software Product Lines",
                "Visualizing Feature-Oriented Software Evolution",
                "Lightweight Visualization of Software Features with HAnS-viz",
                "Towards Decentralized Feature Models"
            };

            var acceptedPapers = await db.Papers
                .Where(p => acceptedTitles.Contains(p.Title))
                .ToListAsync();

            db.Groups.Add(new Group
            {
                Name = "Accepted Papers",
                Description = "Johan Martinson's published and accepted papers across SPLC, TSE, AISoLA, and Variability venues.",
                Papers = acceptedPapers
            });

            await db.SaveChangesAsync();
        }
        // &end[Groups]
    }
}
