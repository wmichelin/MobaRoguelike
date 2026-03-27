UNITY := /Applications/Unity/Hub/Editor/6000.4.0f1/Unity.app/Contents/MacOS/Unity
PROJECT := $(shell pwd)
RESULTS_DIR := TestResults

.PHONY: test test-edit test-play clean-results

test: test-edit test-play

test-edit:
	@mkdir -p $(RESULTS_DIR)
	$(UNITY) -batchmode -nographics -projectPath "$(PROJECT)" \
		-runTests -testPlatform EditMode \
		-testResults "$(RESULTS_DIR)/editmode.xml" \
		-logFile "$(RESULTS_DIR)/editmode.log"
	@echo "EditMode results: $(RESULTS_DIR)/editmode.xml"

test-play:
	@mkdir -p $(RESULTS_DIR)
	$(UNITY) -batchmode -nographics -projectPath "$(PROJECT)" \
		-runTests -testPlatform PlayMode \
		-testResults "$(RESULTS_DIR)/playmode.xml" \
		-logFile "$(RESULTS_DIR)/playmode.log"
	@echo "PlayMode results: $(RESULTS_DIR)/playmode.xml"

clean-results:
	rm -rf $(RESULTS_DIR)
