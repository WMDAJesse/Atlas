Feature: Four out of eight Search - four mismatches
  As a member of the search team
  I want to be able to run a 4/8 cord search
  And see results with four mismatches

  Scenario: 4/8 Search with two mismatches at A, and two at B
    Given a patient and a donor
    And the donor has a double mismatch at locus A
    And the donor has a double mismatch at locus B
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at A, and two at C
    Given a patient and a donor
    And the donor has a double mismatch at locus A
    And the donor has a double mismatch at locus C
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at A, and two at DRB1
    Given a patient and a donor
    And the donor has a double mismatch at locus A
    And the donor has a double mismatch at locus DRB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at A, one at B, and one at C
    Given a patient and a donor
    And the donor has a double mismatch at locus A
    And the donor has a single mismatch at locus B
    And the donor has a single mismatch at locus C
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor
    
  Scenario: 4/8 Search with two mismatches at A, one at B, and one at DRB1
    Given a patient and a donor
    And the donor has a double mismatch at locus A
    And the donor has a single mismatch at locus B
    And the donor has a single mismatch at locus DRB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor
    
  Scenario: 4/8 Search with two mismatches at A, one at C, and one at DRB1
    Given a patient and a donor
    And the donor has a double mismatch at locus A
    And the donor has a single mismatch at locus DRB1
    And the donor has a single mismatch at locus C
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at B, and two at C
    Given a patient and a donor
    And the donor has a double mismatch at locus B
    And the donor has a double mismatch at locus C
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at B, and two at DRB1
    Given a patient and a donor
    And the donor has a double mismatch at locus B
    And the donor has a double mismatch at locus DRB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at B, one at C, and one at DRB1
    Given a patient and a donor
    And the donor has a double mismatch at locus B
    And the donor has a single mismatch at locus DRB1
    And the donor has a single mismatch at locus C
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with two mismatches at C, and two at DRB1
    Given a patient and a donor
    And the donor has a double mismatch at locus C
    And the donor has a double mismatch at locus DRB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor

  Scenario: 4/8 Search with one mismatch each at locus A, B, C, and DRB1
    Given a patient and a donor
    And the donor has a single mismatch at locus A
    And the donor has a single mismatch at locus B
    And the donor has a single mismatch at locus C
    And the donor has a single mismatch at locus DRB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor
    
  Scenario: 4/8 Search with four mismatches at matched loci and mismatches at DQB1
    Given a patient and a donor
    And the donor has a single mismatch at locus A
    And the donor has a single mismatch at locus B
    And the donor has a single mismatch at locus C
    And the donor has a single mismatch at locus DRB1
    And the donor has a double mismatch at locus DQB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor
    
  Scenario: 4/8 Search with four mismatches at matched loci and mismatches at DPB1
    Given a patient and a donor
    And the donor has a single mismatch at locus A
    And the donor has a single mismatch at locus B
    And the donor has a single mismatch at locus C
    And the donor has a single mismatch at locus DRB1
    And the donor has a double mismatch at locus DPB1
    And the donor is of type cord
    And the search type is cord
    When I run a 4/8 search
    Then the results should contain the specified donor
