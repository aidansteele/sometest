package main

import (
	"encoding/xml"
	"github.com/pkg/errors"
	"io"
	"io/ioutil"
	"os"
	"path/filepath"
	"strings"
)

func main() {
	report, err := getReport(os.Args[1])
	if err != nil {
		panic(err)
	}

	printOutput(os.Stdout, *report)
}

func getReport(path string) (*Report, error) {
	path, err := filepath.Abs(path)
	if err != nil {
		return nil, errors.WithStack(err)
	}

	bytes, err := ioutil.ReadFile(path)
	if err != nil {
		return nil, errors.WithStack(err)
	}

	report := Report{}
	err = xml.Unmarshal(bytes, &report)
	if err != nil {
		return nil, errors.WithStack(err)
	}

	return &report, nil
}

func printOutput(w io.Writer, report Report) {
	typeMap := map[string]IssueType{}
	for _, issueType := range report.IssueTypes {
		typeMap[issueType.Id] = issueType
	}

	for _, project := range report.Issues {
		for _, issue := range project.Issues {
			issueType := typeMap[issue.TypeId]
			level := severityToLevel(issueType.Severity)
			column := strings.Split(issue.Offset, "-")[0]
			file := strings.ReplaceAll(issue.File, `\`, "/")
			Message(nil, level, file, issue.Line, column, issue.Message)
		}
	}
}

func severityToLevel(severity string) string {
	switch severity {
	case "WARNING", "ERROR":
		return MessageLevelError
	default:
		return MessageLevelWarning
	}
}
