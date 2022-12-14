namespace EmploymentAgency.EndpointMappers;

using EmploymentAgency.Utils;

public static class SpecialQueriesMapper
{
    public static void Map(WebApplication app, PostgreSql postgres)
    {
        string root = "api/special";
        foreach (
            (string view, bool shouldOrder) in new[]
            {
                ("average_seeker_ages_by_positions", true),
                ("employer_addresses", true),
                ("employment_types_and_salaries", true),
                ("vacancies_and_salaries", true),
                ("employers_and_vacancies", true),
                ("seekers_and_applications", true),
                ("num_vacancies_from_each_employer", true),
                ("total_vacancies_including_not_ended", true),
                ("num_of_seekers_with_university_education", true),
                ("employers_all_and_by_districts", false),
                ("seekers_with_even_average_experience", true)
            }
        )
        {
            app.MapGet(
                $"{root}/{view}",
                async (int page, int pageSize, string? filter) =>
                    await postgres.ReadPageAsync(
                        page,
                        pageSize,
                        Select.From(view),
                        filter,
                        shouldOrder
                    )
            );
        }

        app.MapGet(
            $"{root}/applications_percent_after/{{year}}",
            async (int page, int pageSize, int year, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_applications_percent_after('{year}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/applications_percent_by_positions_after/{{year}}",
            async (int page, int pageSize, int year, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_applications_percent_by_positions_after('{year}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/application_count_by_positions/{{year}}/{{month}}",
            async (int page, int pageSize, int year, int month, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_application_count_by_positions('{year}', '{month}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/seekers_in_district/{{district}}",
            async (int page, int pageSize, string district, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_seekers_in_district('{district}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/employers_with_property/{{property}}",
            async (int page, int pageSize, string property, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_employers_with_property('{property}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/vacancies_posted_on/{{date}}",
            async (int page, int pageSize, DateOnly date, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_vacancies_posted_on('{date}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/seekers_born_after/{{date}}",
            async (int page, int pageSize, DateOnly date, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_seekers_born_after('{date}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/applications_without_experience/{{position}}",
            async (int page, int pageSize, string position, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_applications_without_experience('{position}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/max_salaries_for_position/{{position}}",
            async (int page, int pageSize, string position, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_max_salaries_for_position('{position}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/seekers_whose_total_experience_exceeds/{{experience}}",
            async (int page, int pageSize, int experience, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_seekers_whose_total_experience_exceeds('{experience}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/positions_from_open_vacancies_whose_average_salary_exceeds/{{salary}}",
            async (int page, int pageSize, int salary, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From(
                        $"get_positions_from_open_vacancies_whose_average_salary_exceeds('{salary}')"
                    ),
                    filter
                )
        );

        app.MapGet(
            $"{root}/num_applications_for_each_employment_type",
            async (int page, int pageSize, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_num_applications_for_each_employment_type()"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/latest_vacancy_of_employers_whose_name_contains/{{pattern}}",
            async (int page, int pageSize, string pattern, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From(
                        $"get_latest_vacancy_of_employers_whose_name_contains('{pattern}')"
                    ),
                    filter
                )
        );

        // new
        app.MapGet(
            $"{root}/application_count_of_seekers_whose_name_starts_with/{{chars}}",
            async (int page, int pageSize, string chars, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From(
                        $"get_application_count_of_seekers_whose_name_starts_with('{chars}')"
                    ),
                    filter
                )
        );

        app.MapGet(
            $"{root}/min_salary_of_employer_with_name/{{name}}",
            async (int page, int pageSize, string name, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_min_salary_of_employer_with_name('{name}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/applications_with_position/{{name}}",
            async (int page, int pageSize, string name, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_applications_with_position('{name}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/seekers_not_registered_in/{{city}}",
            async (int page, int pageSize, string city, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_seekers_not_registered_in('{city}')"),
                    filter
                )
        );

        app.MapGet(
            $"{root}/salaries_in_comparison_with/{{salary}}",
            async (int page, int pageSize, int salary, string? filter) =>
                await postgres.ReadPageAsync(
                    page,
                    pageSize,
                    Select.From($"get_salaries_in_comparison_with('{salary}')"),
                    filter
                )
        );
    }
}
