package main

import (
	"encoding/xml"
	"github.com/stretchr/testify/assert"
	"io/ioutil"
	"testing"
)

func TestExample(t *testing.T) {
	bytes, err := ioutil.ReadFile("testdata/inspect.xml")
	assert.NoError(t, err)

	report := Report{}
	err = xml.Unmarshal(bytes, &report)
	assert.NoError(t, err)


}
